using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_posts.Models
{
    public class BlogPostsViewModel
    {
        public List<BlogPost> blogPosts;
        public int currentPage;
        public int totalPages;
    }
    public class BlogViewModel
    {
        public BlogPost BlogPost;
        public List<Comment> comments;
        public string Name;
    }
    public class BlogPostsDb
    {
        private readonly string _connectionString;
        public BlogPostsDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<BlogPost> GetAllBlogPosts(int page)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from blogPosts order by id desc OFFSET @page ROWS FETCH NEXT 3 ROWS ONLY";
            cmd.Parameters.AddWithValue("@page", (page - 1) * 3);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<BlogPost> blogPosts = new();
            while (reader.Read())
            {
                blogPosts.Add(_newBlogPost(reader));
            }
            return blogPosts;
        }
        public BlogPost GetBlogPostForId(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from blogPosts where id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return _newBlogPost(reader);
            }
            return null;
        }
        public List<Comment> GetComments(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from comments where blogPostId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Comment> comments = new();
            while (reader.Read())
            {
                comments.Add(_newComment(reader));
            }
            return comments;
        }
        private BlogPost _newBlogPost(SqlDataReader reader)
        {
            string text = (string)reader["text"];
            string subText;
            if (text.Length > 200)
            {
                subText = text.Substring(0, 200);
                subText += "...";
            }
            else
            {
                subText = text;
            }
            return new BlogPost
            {
                Id = (int)reader["Id"],
                Title = (string)reader["title"],
                Date = (DateTime)reader["date"],
                Text = text,
                SubText = subText
            };
        }
        private Comment _newComment(SqlDataReader reader)
        {
            return new Comment
            {
                Id = (int)reader["Id"],
                Name = (string)reader["name"],
                CommentText = (string)reader["comment"],
                Date = (DateTime)reader["date"],
                BlogPostId = (int)reader["blogPostId"]
            };
        }
        public int GetMostRecentPost()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select  top 1 * from BlogPosts order by Id desc";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            return (int)reader["id"];
        }
        public void AddPost(BlogPost blogPost)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into blogposts values (@Title, @date, @text)";
            cmd.Parameters.AddWithValue("@Title", blogPost.Title);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", blogPost.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddComment(Comment comment)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into comments values (@name, @comment, @blogPostId, @date)";
            cmd.Parameters.AddWithValue("@Name", comment.Name);
            cmd.Parameters.AddWithValue("@comment", comment.CommentText);
            cmd.Parameters.AddWithValue("@blogPostId", comment.BlogPostId);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public int GetTotalPages()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select count(*) from blogposts";
            connection.Open();
            int blogs = (int)cmd.ExecuteScalar();
            int pages = blogs / 3;
            if (blogs % 3 != 0)
            {
                pages++;
            }
            return pages;
        }
    }
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string SubText { get; set; }
    }
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public int BlogPostId { get; set; }
    }
}

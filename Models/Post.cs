
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{
    [Supabase.Postgrest.Attributes.Table("post")]
    public class Post : BaseModel
    {
        [PrimaryKey("postid")]
        public int PostId { get; set; }

        // ID do usuário que criou o post
        [Supabase.Postgrest.Attributes.Column("userid")]
        public Guid UserId { get; set; }

        // Conteúdo do post
        [Supabase.Postgrest.Attributes.Column("content")]
        public string Content { get; set; }

        // PostId do post raiz (se houver)
        [Supabase.Postgrest.Attributes.Column("rootpostid")]
        public int? RootPostId { get; set; }

        // PostId do post pai (se houver)
        [Supabase.Postgrest.Attributes.Column("parentpostid")]
        public int? ParentPostId { get; set; }

        // Contagem de curtidas
        [Supabase.Postgrest.Attributes.Column("likescount")]
        public int LikesCount { get; set; }

        // Contagem de comentários
        [Supabase.Postgrest.Attributes.Column("commentscount")]
        public int CommentsCount { get; set; }

        // Data de criação
        [Supabase.Postgrest.Attributes.Column("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        // Data de atualização
        [Supabase.Postgrest.Attributes.Column("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }

        // Data de exclusão
        [Supabase.Postgrest.Attributes.Column("deletedat")]
        public DateTimeOffset? DeletedAt { get; set; }

        [Reference(typeof(PostAttachment), ReferenceAttribute.JoinType.Left)]
        public List<PostAttachment> Attachments { get; set; }
    }

    [Supabase.Postgrest.Attributes.Table("postattachments")]
    public class PostAttachment : BaseModel
    {
        [PrimaryKey("postattachmentid")]
        public int PostAttachmentId { get; set; }

        // ID do post ao qual o anexo pertence
        [Supabase.Postgrest.Attributes.Column("postid")]
        public int PostId { get; set; }

        // Nome do arquivo
        [Supabase.Postgrest.Attributes.Column("filename")]
        public string Filename { get; set; }

        // Caminho do arquivo
        [Supabase.Postgrest.Attributes.Column("filepath")]
        public string Filepath { get; set; }

        // Tipo do arquivo
        [Supabase.Postgrest.Attributes.Column("filetypeid")]
        public int Filetypeid { get; set; }

        // Data de upload
        [Supabase.Postgrest.Attributes.Column("uploadedat")]
        public DateTimeOffset Uploadedat { get; set; }
    }

    [Supabase.Postgrest.Attributes.Table("postlikes")]
    public class PostLike : BaseModel
    {
        [PrimaryKey("postlikesid")]
        public int PostLikesId { get; set; }

        // ID do post ao qual o anexo pertence        
        [Reference(typeof(Post), ReferenceAttribute.JoinType.Inner)]
        public Post Post { get; set; }

        // ID do usuário que curtiu o post
        [Supabase.Postgrest.Attributes.Column("userid")]
        public Guid UserId { get; set; }

        // ID do post curtido
        [Supabase.Postgrest.Attributes.Column("postid")]
        public int PostId { get; set; }

        // Data de curtida
        [Supabase.Postgrest.Attributes.Column("likedat")]
        public int LikedAt { get; set; }
    }

    public class PostFunction
    {
        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("rootpostid")]
        public int RootPostId { get; set; }

        [JsonPropertyName("parentpostid")]
        public int ParentPostId { get; set; }

        [JsonPropertyName("likescount")]
        public int LikesCount { get; set; }

        [JsonPropertyName("commentscount")]
        public int CommentsCount { get; set; }

        [JsonPropertyName("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
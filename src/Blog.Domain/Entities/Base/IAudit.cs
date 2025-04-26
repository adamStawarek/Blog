namespace Blog.Domain.Entities.Base;
public interface IAudit
{
    string Meta_CreatedBy { get; }
    DateTimeOffset Meta_CreatedDate { get; }
    string Meta_LastModifiedBy { get; }
    DateTimeOffset Meta_LastModifiedDate { get; }

    void SetCreated(string user, DateTimeOffset date);
    void SetUpdated(string user, DateTimeOffset date);
}
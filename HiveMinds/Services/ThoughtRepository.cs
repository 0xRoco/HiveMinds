using HiveMinds.Database;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.API.Services;

public class ThoughtRepository : IThoughtRepository
{

    private readonly DatabaseContext _db;
    private readonly DbSet<Thought> _thoughts;
    private readonly DbSet<ThoughtLike> _thoughtLikes;
    private readonly DbSet<ThoughtReply> _thoughtReplies;
    private readonly ILogger<ThoughtRepository> _logger;
    
    public ThoughtRepository(DatabaseContext db, ILogger<ThoughtRepository> logger)
    {
        _db = db;
        _thoughts = _db.Set<Thought>();
        _thoughtLikes = _db.Set<ThoughtLike>();
        _thoughtReplies = _db.Set<ThoughtReply>();
        _logger = logger;
    }

    public async Task<List<Thought>?> GetThoughts()
    {
        return await _thoughts.ToListAsync();
    }

    public async Task<Thought?> GetThoughtById(int id)
    {
        return await _thoughts.FindAsync(id) ?? null;
    }

    public async Task<List<Thought>?> GetThoughtByUserId(int id)
    {
        return await _thoughts.Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateThought(Thought thought)
    {
        await _thoughts.AddAsync(thought);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateThought(Thought thought)
    {
        _thoughts.Update(thought);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteThought(Thought thought)
    {
        _thoughts.Remove(thought);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<List<ThoughtLike>?> GetLikesByThoughtId(int id)
    {
        return await _thoughtLikes.Where(x => x.ThoughtId == id).ToListAsync();
    }

    public async Task<List<ThoughtLike>?> GetLikesByUserId(int id)
    {
        return await _thoughtLikes.Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateLike(ThoughtLike thoughtLike)
    {
        await _thoughtLikes.AddAsync(thoughtLike);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteLike(ThoughtLike thoughtLike)
    { 
        _thoughtLikes.Remove(thoughtLike);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<List<ThoughtReply>?> GetRepliesByThoughtId(int id)
    {
        return await _thoughtReplies.Where(x => x.ThoughtId == id).ToListAsync();
    }

    public async Task<List<ThoughtReply>?> GetRepliesByUserId(int id)
    {
        return await _thoughtReplies.Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateReply(ThoughtReply thoughtReply)
    {
        await _thoughtReplies.AddAsync(thoughtReply);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateReply(ThoughtReply thoughtReply)
    {
        _thoughtReplies.Update(thoughtReply);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteReply(ThoughtReply thoughtReply)
    {
        _thoughtReplies.Remove(thoughtReply);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }
}
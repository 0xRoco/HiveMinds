using HiveMinds.API.Services.Interfaces;
using HiveMinds.Database;
using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.API.Services;

public class ThoughtRepository : IThoughtRepository
{

    private readonly DatabaseContext _db;
    private readonly ILogger<ThoughtRepository> _logger;
    
    public ThoughtRepository(DatabaseContext db, ILogger<ThoughtRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Thought>?> GetThoughts()
    {
        return await _db.Thought.AsNoTracking().ToListAsync();
    }

    public async Task<Thought?> GetThoughtById(int id)
    {
        return await _db.Thought.FindAsync(id);
    }

    public async Task<List<Thought>?> GetThoughtsByUserId(int id)
    {
        return await _db.Thought.AsNoTracking().Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateThought(Thought thought)
    {
        try
        {
            await _db.Thought.AddAsync(thought);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error creating thought");
            return false;
        }
    }

    public async Task<bool> UpdateThought(Thought thought)
    {
        try
        {
            _db.Thought.Update(thought);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error updating thought");
            return false;
        }
    }

    public async Task<bool> DeleteThought(int thoughtId)
    {
        try
        {
            var thought = await _db.Thought.FindAsync(thoughtId);
            if (thought == null) return false;
            _db.Thought.Remove(thought);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting thought");
            return false;
        }
    }

    public async Task<ThoughtLike?> GetLikeById(int id)
    {
        return await _db.ThoughtLike.FindAsync(id);
    }

    public async Task<List<ThoughtLike>> GetLikesByThoughtId(int id)
    {
        return await _db.ThoughtLike.AsNoTracking().Where(x => x.ThoughtId == id).ToListAsync();
    }

    public async Task<List<ThoughtLike>> GetLikesForUser(int id)
    {
        return await _db.ThoughtLike.AsNoTracking().Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateLike(ThoughtLike thoughtLike)
    {
        try
        {
            await _db.ThoughtLike.AddAsync(thoughtLike);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error creating like");
            return false;
        }
    }

    public async Task<bool> DeleteLike(int thoughtLikeId)
    {
        try
        {
            var thoughtLike = await _db.ThoughtLike.FindAsync(thoughtLikeId);
            if (thoughtLike == null) return false;
            _db.ThoughtLike.Remove(thoughtLike);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting like");
            return false;
        }
    }

    public async Task<ThoughtReply?> GetReplyById(int id)
    {
        return await _db.ThoughtReply.FindAsync(id);
    }

    public async Task<List<ThoughtReply>> GetRepliesByThoughtId(int id)
    {
        return await _db.ThoughtReply.AsNoTracking().Where(x => x.ThoughtId == id).ToListAsync();
    }

    public async Task<List<ThoughtReply>> GetRepliesForUser(int id)
    {
        return await _db.ThoughtReply.AsNoTracking().Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<bool> CreateReply(ThoughtReply thoughtReply)
    {
        try
        {
            await _db.ThoughtReply.AddAsync(thoughtReply);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reply");
            return false;
        }
    }

    public async Task<bool> UpdateReply(ThoughtReply thoughtReply)
    {
        try
        {
            _db.ThoughtReply.Update(thoughtReply);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reply");
            return false;
        }
    }

    public async Task<bool> DeleteReply(int thoughtReplyId)
    {
        try
        {
            var thoughtReply = await _db.ThoughtReply.FindAsync(thoughtReplyId);
            if (thoughtReply == null) return false;
            _db.ThoughtReply.Remove(thoughtReply);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting reply");
            return false;
        }
    }
}
using LibPro.Models;
using Microsoft.EntityFrameworkCore;

namespace LibPro.Services
{
    public class AnnService
    {
        private readonly LibproContext _context;

        public AnnService(LibproContext context)
        {
            _context = context;
        }


        public async Task<List<Announcements>> GetAnnIndex()
        {

            var announcements = await _context.Announcements
                .Include(a => a.Staff)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            return announcements;
        }


        public async Task<Announcements> GetAnnDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var announcements = await _context.Announcements
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.AnnID == id);


            return announcements;
        }



        public async Task<bool> GetAnnCreate(Announcements announcements, string staffID)
        {
            var annIDResult = await _context.Database.SqlQuery<string>($"exec GetAnnID").ToListAsync();
            var newAnnID = annIDResult.FirstOrDefault();

           
            if (string.IsNullOrWhiteSpace(newAnnID))
            {
                throw new Exception("產生 公告編號 失敗，請聯絡管理員。");
            }

            announcements.AnnID = newAnnID;
            announcements.CreatedDate = DateTime.Now;
            announcements.IsVisible = true;
            announcements.Creator = staffID;

            _context.Add(announcements);
            await _context.SaveChangesAsync();

           
            return true;
        }


        public async Task<Announcements> GetAnnEditView(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var announcements = await _context.Announcements.FindAsync(id);

            return announcements;
        }



        public async Task<bool> GetAnnEdit(string id, Announcements announcements)
        {
            var existingAnn = await _context.Announcements.FindAsync(id);


            if (existingAnn == null)
            {
                return false;
            }

            existingAnn.Title = announcements.Title;
            existingAnn.Content = announcements.Content;
            existingAnn.IsVisible = announcements.IsVisible;

            try
            {
                await _context.SaveChangesAsync();
                return true; 
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _context.Announcements.AnyAsync(e => e.AnnID == id);

              
                if (!exists) return false;

                throw;
            }
        }
        public async Task<bool> GetAnnDelete(string id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

           
            if (announcement == null)
            {
                return false;
            }

            announcement.IsVisible = false;

           
            await _context.SaveChangesAsync();

            return true; 
        }

        public bool AnnouncementsExists(string id)
        {
            return _context.Announcements.Any(e => e.AnnID == id);
        }

    }
}

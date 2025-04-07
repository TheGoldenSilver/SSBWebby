using Microsoft.AspNetCore.Mvc.RazorPages;
using GenderApplication.Data;
using GenderApplication.Models;
using System.Threading.Tasks;

public class SignUpModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SignUpModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        // Example of fetching data from the database
        var genders = await _context.Genders.ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync(SignUpModel model)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Example of saving data to the database
        _context.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManage.Data;
using EmployeeManage.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization; // Make sure to add this using statement

namespace EmployeeManage.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Employees
        [Authorize]
        public async Task<IActionResult> Index(string SearchTerm, bool? isActive)
        {
            var employeesQuery = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                employeesQuery = employeesQuery.Where(e =>
                    e.Name.Contains(SearchTerm) ||
                    e.Department.Contains(SearchTerm) ||
                    e.EmailAddress.Contains(SearchTerm));
            }

            if (isActive.HasValue)
            {
                employeesQuery = employeesQuery.Where(e => e.IsActive == isActive.Value);
            }

            var employees = await employeesQuery.ToListAsync();
            var allTasks = await _context.Tasks.ToListAsync();

            var viewModel = employees.Select(emp => new EmployeeWithTaskCountViewModel
            {
                Employee = emp,
                ActiveTask = allTasks.Count(t =>
                    t.AssignedTo == emp.Name && t.Status != "Completed")
            }).ToList();

            ViewData["CurrentFilter"] = SearchTerm;
            ViewData["CurrentIsActive"] = isActive;
           
            return View(viewModel);
        }

        [Authorize]
        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Department,EmailAddress,Phone,Requests,HireDate,ImagePath,IsActive")] Employee employee, IFormFile? EmployeeImageFile )
        {
            if (ModelState.IsValid)
            {
                if(EmployeeImageFile != null && EmployeeImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Employees");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName =
                    Guid.NewGuid().ToString() + "_" +
                    EmployeeImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder,
                    uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await
                        EmployeeImageFile.CopyToAsync(fileStream);
                    }
                    employee.ImagePath = "/images/employees/" +
                    uniqueFileName;

                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Department,EmailAddress,Phone,Requests,HireDate,ImagePath,IsActive")] Employee employee, IFormFile? EmployeeImageFile)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (EmployeeImageFile != null && EmployeeImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Employees");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName =
                        Guid.NewGuid().ToString() + "_" +
                        EmployeeImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder,
                        uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await
                            EmployeeImageFile.CopyToAsync(fileStream);
                        }
                        employee.ImagePath = "/images/employees/" +
                        uniqueFileName;

                    }
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ExportEmployeesToExcel()
    {
        var employees = await
        _context.Employees.ToListAsync();
        using (var package = new ExcelPackage())
        {
            var worksheet =
            package.Workbook.Worksheets.Add("Employees");
            // Add headers
            worksheet.Cells["A1"].Value = "ID";
            worksheet.Cells["B1"].Value = "Name";
            worksheet.Cells["C1"].Value = "Department";
            worksheet.Cells["D1"].Value = "Email";
            worksheet.Cells["E1"].Value = "Phone";
            worksheet.Cells["F1"].Value = "Requests";
            worksheet.Cells["G1"].Value = "Hire Date";
            worksheet.Cells["H1"].Value = "Active";
            // Add data rows
            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                worksheet.Cells[i + 2, 1].Value =
                employee.Id;
                worksheet.Cells[i + 2, 2].Value =
                employee.Name;
                worksheet.Cells[i + 2, 3].Value =
                employee.Department;
                worksheet.Cells[i + 2, 4].Value =
                employee.EmailAddress;
                worksheet.Cells[i + 2, 5].Value =
                employee.Phone;
                worksheet.Cells[i + 2, 6].Value = 
                employee.Requests; 
                worksheet.Cells[i + 2, 7].Value =
                employee.HireDate.ToShortDateString();
                worksheet.Cells[i + 2, 8].Value =
                employee.IsActive ? "Yes" : "No";
            }
            // AutoFit columns for better readability
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            string excelName =
            $"Employees_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
}
    }

}
}

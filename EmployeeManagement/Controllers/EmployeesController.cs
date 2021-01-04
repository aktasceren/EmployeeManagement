using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly EmployeeContext _context;

        public EmployeesController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: Employees
        public IActionResult Index(int page = 1, int pageSize = 3)
        {
            PagedList<Employee> model = new PagedList<Employee>(_context.Employee.Include(i =>i.Department), page, pageSize);
            return View("Index", model);
        }


        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.ID == id);
            employee.Department = _context.Department.Find(employee.DepartmentId);
            var dep = employee.Department.DepartmentName;
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        
        // GET: Employees/Create
        public IActionResult Create()
        {
            List<Department> DepartmentList = new List<Department>();

            DepartmentList = _context.Department.ToList();

            DepartmentList.Insert(0, new Department { DepartmentId= 0, DepartmentName = "Select"});

            ViewBag.ListOfDepartments = DepartmentList;
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                var dep = _context.Department.Where(d => d.DepartmentId == emp.DepartmentId).FirstOrDefault();
                Employee employee = new Employee { Name = emp.Name, Surname = emp.Surname, Email = emp.Email, Department = dep, DepartmentId = emp.DepartmentId, PhoneNumber = emp.PhoneNumber, Birthday = emp.Birthday, StartDate = emp.StartDate };
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emp);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            List<Department> DepartmentList = new List<Department>();

            DepartmentList = _context.Department.ToList();

            DepartmentList.Insert(0, new Department { DepartmentId = 0, DepartmentName = "Select" });

            ViewBag.ListOfDepartments = DepartmentList;
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,Email,PhoneNumber,DepartmentId,StartDate,Birthday")] Employee employee)
        {
            if (id != employee.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.ID))
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.ID == id);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Infrastructure.Data.Seed;

public static class DbSeeder
{
    private const string DefaultStudentPin = "1234";
    private const string DefaultAdminPassword = "Admin@123";

    public static async Task SeedAsync(AppDbContext db)
    {
        // Idempotency guard
        if (await db.Colleges.AnyAsync())
            return;

        await SeedCollegesAndDepartmentsAsync(db);
        await SeedStudyPlansAsync(db);
        await SeedCoursesAsync(db);
        await SeedStudyPlanCoursesAsync(db);
        await SeedPrerequisitesAsync(db);
        await SeedSemestersAndPeriodsAsync(db);
        await SeedInstructorsAsync(db);
        await SeedStudentsAsync(db);
        await SeedSectionsAsync(db);
        await SeedAdminAsync(db);
    }

    // ============================================================
    // 1. COLLEGES + DEPARTMENTS
    // ============================================================
    private static async Task SeedCollegesAndDepartmentsAsync(AppDbContext db)
    {
        var eng = new College { Name = "College of Engineering and Technology", Code = "ENG" };
        var mgt = new College { Name = "College of Management and Technology", Code = "MGT" };
        db.Colleges.AddRange(eng, mgt);
        await db.SaveChangesAsync();

        db.Departments.AddRange(
            new Department { Name = "Electronics and Communications Engineering", Code = "ECE", CollegeId = eng.Id },
            new Department { Name = "Computer Engineering", Code = "CPE", CollegeId = eng.Id },
            new Department { Name = "Marine Engineering", Code = "MRE", CollegeId = eng.Id },
            new Department { Name = "Mechatronics Engineering", Code = "MTR", CollegeId = eng.Id },
            new Department { Name = "Biomedical Engineering", Code = "BME", CollegeId = eng.Id },
            new Department { Name = "Aerospace Engineering", Code = "ASE", CollegeId = eng.Id },
            new Department { Name = "Business Administration", Code = "BUA", CollegeId = mgt.Id },
            new Department { Name = "Marketing", Code = "MKT", CollegeId = mgt.Id },
            new Department { Name = "Accounting", Code = "ACC", CollegeId = mgt.Id },
            new Department { Name = "Finance", Code = "FIN", CollegeId = mgt.Id },
            new Department { Name = "Business Information Systems", Code = "BIS", CollegeId = mgt.Id }
        );
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 2. STUDY PLANS
    // ============================================================
    private static async Task SeedStudyPlansAsync(AppDbContext db)
    {
        var depts = await db.Departments.ToDictionaryAsync(d => d.Code, d => d.Id);

        db.StudyPlans.AddRange(depts.Select(kv => new StudyPlan
        {
            Name = $"{kv.Key} Master's Plan 2024",
            Year = 2024,
            Level = StudentLevel.Master,
            DepartmentId = kv.Value
        }));
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 3. COURSES
    // ============================================================
    private static async Task SeedCoursesAsync(AppDbContext db)
    {
        var depts = await db.Departments.ToDictionaryAsync(d => d.Code, d => d.Id);

        db.Courses.AddRange(CourseCatalog.All.Select(c => new Course
        {
            Code = c.Code,
            Name = c.Name,
            CreditHours = c.CH,
            DepartmentId = depts[c.DeptCode]
        }));
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 4. STUDY PLAN COURSES
    // ============================================================
    private static async Task SeedStudyPlanCoursesAsync(AppDbContext db)
    {
        var plans = await db.StudyPlans.Include(p => p.Department)
            .ToDictionaryAsync(p => p.Department.Code, p => p.Id);
        var courses = await db.Courses.ToDictionaryAsync(c => c.Code, c => c.Id);

        var toAdd = new List<StudyPlanCourse>();
        foreach (var (deptCode, termMap) in StudyPlanCatalog.ByDepartment)
        {
            if (!plans.TryGetValue(deptCode, out var planId)) continue;

            foreach (var (term, codes) in termMap)
                foreach (var code in codes)
                    if (courses.TryGetValue(code, out var cid))
                        toAdd.Add(new StudyPlanCourse
                        {
                            StudyPlanId = planId,
                            CourseId = cid,
                            SuggestedTerm = term,
                            IsMandatory = true
                        });
        }

        db.StudyPlanCourses.AddRange(toAdd);
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 5. PREREQUISITES
    // ============================================================
    private static async Task SeedPrerequisitesAsync(AppDbContext db)
    {
        var courses = await db.Courses.ToDictionaryAsync(c => c.Code, c => c.Id);

        var toAdd = new List<CoursePrerequisite>();
        foreach (var (courseCode, prereqCodes) in StudyPlanCatalog.Prerequisites)
        {
            if (!courses.TryGetValue(courseCode, out var cid)) continue;

            foreach (var p in prereqCodes)
                if (courses.TryGetValue(p, out var pid))
                    toAdd.Add(new CoursePrerequisite { CourseId = cid, PrerequisiteCourseId = pid });
        }

        db.CoursePrerequisites.AddRange(toAdd);
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 6. SEMESTERS + REGISTRATION PERIODS
    // ============================================================
    private static async Task SeedSemestersAndPeriodsAsync(AppDbContext db)
    {
        var current = new Semester
        {
            Name = "First 2025/2026",
            StartDate = DateTime.UtcNow.Date.AddDays(-30),
            EndDate = DateTime.UtcNow.Date.AddDays(90),
            IsCurrent = true
        };
        var past = new Semester
        {
            Name = "Second 2024/2025",
            StartDate = new DateTime(2025, 2, 1),
            EndDate = new DateTime(2025, 5, 31),
            IsCurrent = false
        };
        db.Semesters.AddRange(current, past);
        await db.SaveChangesAsync();

        var depts = await db.Departments.ToListAsync();
        foreach (var d in depts)
            db.RegistrationPeriods.Add(new RegistrationPeriod
            {
                DepartmentId = d.Id,
                SemesterId = current.Id,
                StartDate = DateTime.UtcNow.Date.AddDays(-7),
                EndDate = DateTime.UtcNow.Date.AddDays(7),
                IsOpen = true
            });
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 7. INSTRUCTORS + TEACHING ASSISTANTS
    // ============================================================
    private static async Task SeedInstructorsAsync(AppDbContext db)
    {
        db.Instructors.AddRange(
            new Instructor { Name = "Dr. Ahmed Hassan", Email = "a.hassan@example.edu" },
            new Instructor { Name = "Dr. Sara Mohamed", Email = "s.mohamed@example.edu" },
            new Instructor { Name = "Dr. Khaled Ibrahim", Email = "k.ibrahim@example.edu" },
            new Instructor { Name = "Dr. Mona El-Sayed", Email = "m.elsayed@example.edu" },
            new Instructor { Name = "Dr. Tarek Nabil", Email = "t.nabil@example.edu" },
            new Instructor { Name = "Dr. Nour Abdelrahman", Email = "n.abdelrahman@example.edu" },
            new Instructor { Name = "Dr. Hany Farouk", Email = "h.farouk@example.edu" },
            new Instructor { Name = "Dr. Rania Samir", Email = "r.samir@example.edu" }
        );
        await db.SaveChangesAsync();

        db.TeachingAssistants.AddRange(
            new TeachingAssistant { Name = "Eng. Omar Youssef", Email = "o.youssef@example.edu" },
            new TeachingAssistant { Name = "Eng. Yasmine Adel", Email = "y.adel@example.edu" },
            new TeachingAssistant { Name = "Eng. Karim Fathy", Email = "k.fathy@example.edu" },
            new TeachingAssistant { Name = "Eng. Dalia Mahmoud", Email = "d.mahmoud@example.edu" }
        );
        await db.SaveChangesAsync();
    }

    // ============================================================
    // 8. STUDENTS (Master) + COMPLETED COURSES
    // ============================================================
    private static async Task SeedStudentsAsync(AppDbContext db)
    {
        var depts = await db.Departments.ToDictionaryAsync(d => d.Code, d => d.Id);
        var plans = await db.StudyPlans.Include(p => p.Department)
            .ToDictionaryAsync(p => p.Department.Code, p => p.Id);
        var courses = await db.Courses.ToDictionaryAsync(c => c.Code, c => c.Id);

        var pinHash = BCrypt.Net.BCrypt.HashPassword(DefaultStudentPin);

        Student Make(string reg, string name, string deptCode, StudentStatus status, decimal? gpa = 3.5m)
            => new()
            {
                RegistrationNumber = reg,
                FullName = name,
                PinHash = pinHash,
                DepartmentId = depts[deptCode],
                StudyPlanId = plans[deptCode],
                Status = status,
                Level = StudentLevel.Master,
                GPA = gpa,
                TotalAchievement = 80m,
                Sponsor = "Self"
            };

        var students = new[]
        {
            Make("20260001", "Ahmed Mahmoud",   "ECE", StudentStatus.Active),     // Term 3 - full prereqs
            Make("20260002", "Sara Ali",        "ECE", StudentStatus.Active),     // Term 5 - full prereqs
            Make("20260003", "Mohamed Khaled",  "CPE", StudentStatus.Active),     // Term 3 - full prereqs
            Make("20260004", "Fatma Hassan",    "CPE", StudentStatus.Active),     // Term 5 - full prereqs
            Make("20260005", "Omar Ibrahim",    "BUA", StudentStatus.Active),     // Term 2
            Make("20260006", "Layla Ahmed",     "BUA", StudentStatus.Active),     // Term 4
            Make("20260007", "Youssef Tarek",   "ACC", StudentStatus.Active),     // Term 2
            Make("20260008", "Nour Mostafa",    "ACC", StudentStatus.Active),     // Term 4
            Make("20260009", "Ali Reda",        "CPE", StudentStatus.Active),     // Term 3 - MISSING CC112 (prereq test)
            Make("20260010", "Khaled Sami",     "ECE", StudentStatus.Graduated),
            Make("20260011", "Hana Yasser",     "ECE", StudentStatus.Suspended),
            Make("20260012", "Ziad Ayman",      "CPE", StudentStatus.Dismissed),
        };
        db.Students.AddRange(students);
        await db.SaveChangesAsync();

        // ============================================================
        // Completed courses per student
        // ============================================================
        var s = students.ToDictionary(x => x.RegistrationNumber, x => x.Id);

        void Done(string reg, params string[] codes)
        {
            foreach (var code in codes)
                if (courses.TryGetValue(code, out var cid))
                    db.StudentCompletedCourses.Add(new StudentCompletedCourse
                    {
                        StudentId = s[reg],
                        CourseId = cid,
                        Grade = "A",
                        CompletedAt = new DateTime(2025, 6, 1)
                    });
        }

        // Ahmed ECE (Term 3) — completed Terms 1+2
        Done("20260001", "BA113", "BA123", "BA141", "CC111", "IM111", "LH131", "ME151",
                          "BA114", "BA118", "BA124", "BA142", "CC112", "IM112", "LH132");

        // Sara ECE (Term 5) — completed Terms 1-4
        Done("20260002", "BA113", "BA123", "BA141", "CC111", "IM111", "LH131", "ME151",
                          "BA114", "BA118", "BA124", "BA142", "CC112", "IM112", "LH132",
                          "BA223", "CC213", "EC210", "EE231", "LH231", "NE465",
                          "BA224", "CC216", "EC217", "EC233", "EE232", "NE264");

        // Mohamed CPE (Term 3) — completed Terms 1+2 (CPE)
        Done("20260003", "BA113", "BA118", "BA123", "BA141", "CC111", "IM111", "LH131",
                          "BA114", "BA124", "BA142", "CC112", "IM112", "LH132", "ME151");

        // Fatma CPE (Term 5) — completed Terms 1-4 (CPE)
        Done("20260004", "BA113", "BA118", "BA123", "BA141", "CC111", "IM111", "LH131",
                          "BA114", "BA124", "BA142", "CC112", "IM112", "LH132", "ME151",
                          "BA223", "CC212", "CC218", "EE231", "LH231",
                          "BA224", "CC215", "CC216", "EC218", "EC238", "EE232");

        // Omar BUA (Term 2) — completed Term 1
        Done("20260005", "MGT101", "STA101", "MKT101", "ACC101", "BUS101", "ECO101");

        // Layla BUA (Term 4) — completed Terms 1-3
        Done("20260006", "MGT101", "STA101", "MKT101", "ACC101", "BUS101", "ECO101",
                          "MGT102", "ECO102", "LAW101", "ACC102", "BUS102", "FIN201",
                          "MGT201", "MGT202", "ACC201", "BUS201", "MIS201");

        // Youssef ACC (Term 2) — completed Term 1
        Done("20260007", "ACC101", "STA101", "ECO101", "MGT101", "BUS101", "LAW101");

        // Nour ACC (Term 4) — completed Terms 1-3
        Done("20260008", "ACC101", "STA101", "ECO101", "MGT101", "BUS101", "LAW101",
                          "ACC102", "ACC201", "ECO102", "ACC202", "BUS102",
                          "ACC301", "ACC302", "ACC303", "FIN201", "BUS201");

        // Ali CPE (Term 3) — completed Terms 1+2 EXCEPT CC112 (intentional prereq test)
        Done("20260009", "BA113", "BA118", "BA123", "BA141", "CC111", "IM111", "LH131",
                          "BA114", "BA124", "BA142", "IM112", "LH132", "ME151");

        await db.SaveChangesAsync();
    }

    // ============================================================
    // 9. SECTIONS + SCHEDULES
    // Designed so that:
    //   - Sara (ECE Term 5) can pick up to 9 non-conflicting courses = 27 CH
    //   - BA223 Sec1 + CC213 Sec1 CONFLICT on Sunday 10-12
    //   - EC210 is FULL
    // ============================================================
    private static async Task SeedSectionsAsync(AppDbContext db)
    {
        var sem = await db.Semesters.FirstAsync(x => x.IsCurrent);
        var courses = await db.Courses.ToDictionaryAsync(c => c.Code, c => c.Id);
        var insts = await db.Instructors.OrderBy(i => i.Id).ToListAsync();
        var tas = await db.TeachingAssistants.OrderBy(t => t.Id).ToListAsync();

        var templates = new SectionTpl[]
        {
            // ===== ECE Term 3 =====
            // BA223 Sec1 (Sun 10-12, Tue 10-12, Thu 14-16) — CONFLICT with CC213 Sec1 (Sun 10-12)
            new("BA223", "1", 20, 0, 0, 0,
                (WeekDay.Sunday, 10, 12), (WeekDay.Tuesday, 10, 12), (WeekDay.Thursday, 14, 16)),
            // BA223 Sec2 (Mon 14-16, Wed 14-16, Sat 10-12) — no conflict
            new("BA223", "2", 20, 0, 1, 1,
                (WeekDay.Monday, 14, 16), (WeekDay.Wednesday, 14, 16), (WeekDay.Saturday, 10, 12)),
            // CC213 Sec1 (Sun 10-12, Wed 10-12, Mon 14-16) — CONFLICT with BA223 Sec1
            new("CC213", "1", 25, 0, 2, 2,
                (WeekDay.Sunday, 10, 12), (WeekDay.Wednesday, 10, 12), (WeekDay.Monday, 14, 16)),
            // CC212 Sec1
            new("CC212", "1", 25, 0, 3, 3,
                (WeekDay.Sunday, 8, 10), (WeekDay.Tuesday, 8, 10), (WeekDay.Thursday, 8, 10)),
            // CC218 Sec1
            new("CC218", "1", 30, 0, 0, null,
                (WeekDay.Monday, 8, 10), (WeekDay.Wednesday, 8, 10), (WeekDay.Saturday, 8, 10)),
            // EE231 Sec1
            new("EE231", "1", 25, 0, 1, 0,
                (WeekDay.Sunday, 12, 14), (WeekDay.Tuesday, 12, 14), (WeekDay.Thursday, 12, 14)),
            // LH231 Sec1
            new("LH231", "1", 40, 0, 4, null,
                (WeekDay.Monday, 10, 12), (WeekDay.Wednesday, 12, 14)),
            // NE465 Sec1
            new("NE465", "1", 40, 0, 5, null,
                (WeekDay.Thursday, 10, 12), (WeekDay.Saturday, 14, 16)),
            // EC210 Sec1 — FULL
            new("EC210", "1", 20, 20, 2, 1,
                (WeekDay.Monday, 12, 14), (WeekDay.Wednesday, 12, 14), (WeekDay.Saturday, 12, 14)),

            // ===== ECE Term 5 — REDESIGNED FOR SARA (NO CONFLICTS AMONG ALL) =====
            // BA323 (Sun 8-10, Tue 8-10, Thu 8-10)
            new("BA323", "1", 25, 0, 0, null,
                (WeekDay.Sunday, 8, 10), (WeekDay.Tuesday, 8, 10), (WeekDay.Thursday, 8, 10)),
            // CC312 (Sun 10-12, Wed 10-12)  ← moved to avoid EE328 & EC334
            new("CC312", "1", 25, 0, 1, 2,
                (WeekDay.Sunday, 10, 12), (WeekDay.Wednesday, 10, 12)),
            // EC321M (Sun 12-14, Tue 12-14)
            new("EC321M", "1", 25, 0, 4, 1,
                (WeekDay.Sunday, 12, 14), (WeekDay.Tuesday, 12, 14)),
            // EC332 (Mon 8-10, Wed 8-10)
            new("EC332", "1", 25, 0, 6, 2,
                (WeekDay.Monday, 8, 10), (WeekDay.Wednesday, 8, 10)),
            // EC334 (Mon 10-12, Thu 10-12)
            new("EC334", "1", 25, 0, 0, 0,
                (WeekDay.Monday, 10, 12), (WeekDay.Thursday, 10, 12)),
            // EE328 (Tue 14-16, Thu 14-16)
            new("EE328", "1", 25, 0, 1, 1,
                (WeekDay.Tuesday, 14, 16), (WeekDay.Thursday, 14, 16)),

            // ===== ECE Term 6 — ADDITIONAL 9 CH WITHOUT CONFLICTS =====
            // CC413 (Mon 12-14, Wed 12-14)
            new("CC413", "1", 25, 0, 2, null,
                (WeekDay.Monday, 12, 14), (WeekDay.Wednesday, 12, 14)),
            // EC311 (Sun 14-16, Tue 16-18)
            new("EC311", "1", 25, 0, 3, null,
                (WeekDay.Sunday, 14, 16), (WeekDay.Tuesday, 16, 18)),
            // EC341 (Thu 12-14, Sat 8-10)
            new("EC341", "1", 25, 0, 4, null,
                (WeekDay.Thursday, 12, 14), (WeekDay.Saturday, 8, 10)),

            // ===== CPE Term 5 =====
            new("CC317", "1", 25, 0, 2, 3,
                (WeekDay.Monday, 10, 12), (WeekDay.Wednesday, 10, 12), (WeekDay.Thursday, 14, 16)),
            new("CC319", "1", 30, 0, 3, 0,
                (WeekDay.Tuesday, 10, 12), (WeekDay.Thursday, 10, 12), (WeekDay.Saturday, 8, 10)),
            new("EC320", "1", 25, 0, 5, null,
                (WeekDay.Thursday, 10, 12), (WeekDay.Monday, 14, 16)),
            new("EC339", "1", 25, 0, 7, 3,
                (WeekDay.Monday, 12, 14), (WeekDay.Wednesday, 14, 16), (WeekDay.Saturday, 14, 16)),

            // ===== BUA courses =====
            new("MGT102", "1", 30, 0, 0, null,
                (WeekDay.Sunday, 8, 10), (WeekDay.Tuesday, 8, 10)),
            new("MGT201", "1", 30, 0, 1, 0,
                (WeekDay.Sunday, 10, 12), (WeekDay.Wednesday, 10, 12)),
            new("MGT202", "1", 30, 0, 2, null,
                (WeekDay.Monday, 8, 10), (WeekDay.Thursday, 10, 12)),
            new("MGT301", "1", 30, 0, 3, 1,
                (WeekDay.Monday, 10, 12), (WeekDay.Wednesday, 12, 14)),
            new("MGT302", "1", 30, 0, 4, null,
                (WeekDay.Tuesday, 8, 10), (WeekDay.Saturday, 10, 12)),
            new("MGT303", "1", 30, 0, 5, null,
                (WeekDay.Tuesday, 10, 12), (WeekDay.Thursday, 12, 14)),
            new("MGT304", "1", 30, 0, 6, null,
                (WeekDay.Wednesday, 8, 10), (WeekDay.Saturday, 12, 14)),
            new("ECO102", "1", 30, 0, 7, null,
                (WeekDay.Sunday, 12, 14), (WeekDay.Tuesday, 14, 16)),
            new("ACC102", "1", 30, 0, 0, 2,
                (WeekDay.Sunday, 8, 10), (WeekDay.Monday, 14, 16)),
            new("ACC201", "1", 30, 0, 1, 3,
                (WeekDay.Monday, 8, 10), (WeekDay.Wednesday, 8, 10)),
            new("FIN201", "1", 30, 0, 2, null,
                (WeekDay.Monday, 10, 12), (WeekDay.Tuesday, 14, 16)),
            new("BUS102", "1", 40, 0, 3, null,
                (WeekDay.Tuesday, 8, 10), (WeekDay.Thursday, 14, 16)),
            new("LAW101", "1", 40, 0, 4, null,
                (WeekDay.Tuesday, 10, 12), (WeekDay.Saturday, 12, 14)),
            new("MIS201", "1", 30, 0, 5, 0,
                (WeekDay.Wednesday, 8, 10), (WeekDay.Saturday, 10, 12)),
            new("MKT301", "1", 30, 0, 6, null,
                (WeekDay.Wednesday, 10, 12), (WeekDay.Saturday, 14, 16)),
            new("BUS201", "1", 30, 0, 7, null,
                (WeekDay.Thursday, 8, 10), (WeekDay.Monday, 16, 18)),

            // ===== ACC courses =====
            new("ACC202", "1", 30, 0, 0, null,
                (WeekDay.Sunday, 12, 14), (WeekDay.Wednesday, 14, 16)),
            new("ACC301", "1", 25, 0, 1, 1,
                (WeekDay.Sunday, 8, 10), (WeekDay.Tuesday, 10, 12)),
            new("ACC302", "1", 25, 0, 2, null,
                (WeekDay.Monday, 8, 10), (WeekDay.Thursday, 10, 12)),
            new("ACC303", "1", 25, 0, 3, null,
                (WeekDay.Monday, 10, 12), (WeekDay.Wednesday, 12, 14)),
            new("ACC401", "1", 25, 0, 4, 2,
                (WeekDay.Sunday, 10, 12), (WeekDay.Tuesday, 14, 16)),
            new("ACC402", "1", 25, 0, 5, null,
                (WeekDay.Tuesday, 8, 10), (WeekDay.Saturday, 10, 12)),
            new("ACC403", "1", 25, 0, 6, null,
                (WeekDay.Wednesday, 8, 10), (WeekDay.Saturday, 14, 16)),
        };

        foreach (var t in templates)
        {
            if (!courses.TryGetValue(t.CourseCode, out var cid)) continue;

            var section = new CourseSection
            {
                CourseId = cid,
                SemesterId = sem.Id,
                SectionNumber = t.Section,
                Capacity = t.Capacity,
                EnrolledCount = t.Enrolled,
                InstructorId = insts[t.InstrIdx].Id,
                TAId = t.TAIdx.HasValue ? tas[t.TAIdx.Value].Id : null
            };

            for (int i = 0; i < t.Slots.Length; i++)
            {
                var (day, sh, eh) = t.Slots[i];
                section.Schedules.Add(new SectionSchedule
                {
                    DayOfWeek = day,
                    StartTime = new TimeOnly(sh, 0),
                    EndTime = new TimeOnly(eh, 0),
                    Room = i == 0 ? $"Hall-{t.CourseCode}" : $"Lab-{t.CourseCode}"
                });
            }

            db.CourseSections.Add(section);
        }

        await db.SaveChangesAsync();
    }

    // ============================================================
    // 10. ADMIN
    // ============================================================
    private static async Task SeedAdminAsync(AppDbContext db)
    {
        db.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultAdminPassword),
            Role = "Admin"
        });
        await db.SaveChangesAsync();
    }

    // ============================================================
    // Helper record
    // ============================================================
    private record SectionTpl(
        string CourseCode,
        string Section,
        int Capacity,
        int Enrolled,
        int InstrIdx,
        int? TAIdx,
        params (WeekDay Day, int Start, int End)[] Slots);
}
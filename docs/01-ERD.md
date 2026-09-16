erDiagram
Department ||--o{ Student : "has"
Department ||--o{ StudyPlan : "offers"
Department ||--o{ Course : "belongs to"
Department ||--o{ RegistrationPeriod : "opens for"

    StudyPlan ||--o{ StudyPlanCourse : "contains"
    Course ||--o{ StudyPlanCourse : "included in"

    Course ||--o{ CoursePrerequisite : "has prerequisite"
    Course ||--o{ CoursePrerequisite : "is prerequisite of"

    Course ||--o{ CourseSection : "has sections"
    Semester ||--o{ CourseSection : "offered in"
    Semester ||--o{ RegistrationPeriod : "applies to"

    Instructor ||--o{ CourseSection : "teaches"
    TeachingAssistant ||--o{ CourseSection : "assists"

    CourseSection ||--o{ SectionSchedule : "scheduled at"
    CourseSection ||--o{ Enrollment : "enrolls"

    Student ||--o{ Enrollment : "registers"
    Student ||--o{ StudentCompletedCourse : "completed"
    Course ||--o{ StudentCompletedCourse : "completed by"

    Student }o--|| StudyPlan : "follows"
    Student }o--|| Department : "belongs to"

    Department {
        int Id PK
        string Name
        string Code UK
    }

    Student {
        int Id PK
        string RegistrationNumber UK
        string FullName
        string PinHash
        int DepartmentId FK
        int StudyPlanId FK
        int Status
        decimal TotalAchievement
        decimal GPA
        string Sponsor
    }

    StudyPlan {
        int Id PK
        string Name
        int DepartmentId FK
        int Year
    }

    StudyPlanCourse {
        int StudyPlanId PK,FK
        int CourseId PK,FK
        int SuggestedSemester
        bool IsMandatory
    }

    Course {
        int Id PK
        string Code UK
        string Name
        int CreditHours
        int DepartmentId FK
    }

    CoursePrerequisite {
        int CourseId PK,FK
        int PrerequisiteCourseId PK,FK
    }

    Semester {
        int Id PK
        string Name
        datetime StartDate
        datetime EndDate
        bool IsCurrent
    }

    RegistrationPeriod {
        int Id PK
        int DepartmentId FK
        int SemesterId FK
        datetime StartDate
        datetime EndDate
        bool IsOpen
    }

    Instructor {
        int Id PK
        string Name
        string Email
    }

    TeachingAssistant {
        int Id PK
        string Name
        string Email
    }

    CourseSection {
        int Id PK
        int CourseId FK
        int SemesterId FK
        string SectionNumber
        int Capacity
        int EnrolledCount
        int InstructorId FK
        int TAId FK
    }

    SectionSchedule {
        int Id PK
        int SectionId FK
        int DayOfWeek
        time StartTime
        time EndTime
        string Room
    }

    Enrollment {
        int Id PK
        int StudentId FK
        int SectionId FK
        int Status
        datetime RegisteredAt
    }

    StudentCompletedCourse {
        int StudentId PK,FK
        int CourseId PK,FK
        string Grade
        datetime CompletedAt
    }

    AdminUser {
        int Id PK
        string Username UK
        string PasswordHash
        string Role
    }

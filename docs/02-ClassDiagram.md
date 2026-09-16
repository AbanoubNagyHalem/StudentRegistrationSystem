classDiagram
class Student {
+int Id
+string RegistrationNumber
+string FullName
+string PinHash
+StudentStatus Status
+decimal TotalAchievement
+decimal GPA
+string Sponsor
+int DepartmentId
+int StudyPlanId
+Department Department
+StudyPlan StudyPlan
+ICollection~Enrollment~ Enrollments
+ICollection~StudentCompletedCourse~ CompletedCourses
+bool IsEligibleForRegistration()
}

    class Department {
        +int Id
        +string Name
        +string Code
        +ICollection~Student~ Students
        +ICollection~StudyPlan~ StudyPlans
        +ICollection~Course~ Courses
        +ICollection~RegistrationPeriod~ RegistrationPeriods
    }

    class StudyPlan {
        +int Id
        +string Name
        +int Year
        +int DepartmentId
        +Department Department
        +ICollection~StudyPlanCourse~ Courses
    }

    class StudyPlanCourse {
        +int StudyPlanId
        +int CourseId
        +int SuggestedSemester
        +bool IsMandatory
        +StudyPlan StudyPlan
        +Course Course
    }

    class Course {
        +int Id
        +string Code
        +string Name
        +int CreditHours
        +int DepartmentId
        +Department Department
        +ICollection~CoursePrerequisite~ Prerequisites
        +ICollection~CourseSection~ Sections
    }

    class CoursePrerequisite {
        +int CourseId
        +int PrerequisiteCourseId
        +Course Course
        +Course PrerequisiteCourse
    }

    class Semester {
        +int Id
        +string Name
        +DateTime StartDate
        +DateTime EndDate
        +bool IsCurrent
        +ICollection~CourseSection~ Sections
    }

    class RegistrationPeriod {
        +int Id
        +int DepartmentId
        +int SemesterId
        +DateTime StartDate
        +DateTime EndDate
        +bool IsOpen
        +bool IsActive()
    }

    class Instructor {
        +int Id
        +string Name
        +string Email
        +ICollection~CourseSection~ Sections
    }

    class TeachingAssistant {
        +int Id
        +string Name
        +string Email
        +ICollection~CourseSection~ Sections
    }

    class CourseSection {
        +int Id
        +int CourseId
        +int SemesterId
        +string SectionNumber
        +int Capacity
        +int EnrolledCount
        +int InstructorId
        +int TAId
        +Course Course
        +Semester Semester
        +Instructor Instructor
        +TeachingAssistant TeachingAssistant
        +ICollection~SectionSchedule~ Schedules
        +ICollection~Enrollment~ Enrollments
        +bool HasAvailableSeats()
    }

    class SectionSchedule {
        +int Id
        +int SectionId
        +DayOfWeek DayOfWeek
        +TimeOnly StartTime
        +TimeOnly EndTime
        +string Room
        +CourseSection Section
    }

    class Enrollment {
        +int Id
        +int StudentId
        +int SectionId
        +EnrollmentStatus Status
        +DateTime RegisteredAt
        +Student Student
        +CourseSection Section
    }

    class StudentCompletedCourse {
        +int StudentId
        +int CourseId
        +string Grade
        +DateTime CompletedAt
        +Student Student
        +Course Course
    }

    class AdminUser {
        +int Id
        +string Username
        +string PasswordHash
        +string Role
    }

    Student "1" --> "*" Enrollment
    Student "1" --> "*" StudentCompletedCourse
    Student "*" --> "1" Department
    Student "*" --> "0..1" StudyPlan
    Department "1" --> "*" Student
    Department "1" --> "*" StudyPlan
    Department "1" --> "*" Course
    Department "1" --> "*" RegistrationPeriod
    StudyPlan "1" --> "*" StudyPlanCourse
    Course "1" --> "*" StudyPlanCourse
    Course "1" --> "*" CoursePrerequisite
    Course "1" --> "*" CourseSection
    Semester "1" --> "*" CourseSection
    Instructor "1" --> "*" CourseSection
    TeachingAssistant "1" --> "*" CourseSection
    CourseSection "1" --> "*" SectionSchedule
    CourseSection "1" --> "*" Enrollment
    Course "1" --> "*" StudentCompletedCourse

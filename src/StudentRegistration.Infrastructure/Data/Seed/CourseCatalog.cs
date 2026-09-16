namespace StudentRegistration.Infrastructure.Data.Seed;

/// <summary>
/// Static catalog of all courses used in the seed.
/// Codes are unique across the system.
/// (Code, Name, CreditHours, HomeDepartmentCode)
/// </summary>
internal static class CourseCatalog
{
    public static readonly (string Code, string Name, int CH, string DeptCode)[] All =
    {
        // ============================================================
        // Basic Sciences (BA) — Engineering
        // ============================================================
        ("BA113", "Physics I", 3, "ECE"),
        ("BA114", "Physics II", 3, "ECE"),
        ("BA118", "Chemistry", 3, "ECE"),
        ("BA123", "Mathematics I", 3, "ECE"),
        ("BA124", "Mathematics II", 3, "ECE"),
        ("BA141", "Engineering Mechanics I", 3, "MRE"),
        ("BA142", "Engineering Mechanics II", 3, "MRE"),
        ("BA223", "Mathematics III", 3, "ECE"),
        ("BA224", "Mathematics IV", 3, "ECE"),
        ("BA323", "Mathematics V", 3, "ECE"),
        ("BA326", "Mathematics VI - Probability and Statistics", 3, "CPE"),

        // ============================================================
        // Computers (CC) — Engineering
        // ============================================================
        ("CC111", "Introduction to Computers", 3, "CPE"),
        ("CC112", "Structured Programming", 3, "CPE"),
        ("CC212", "Applied Programming", 3, "CPE"),
        ("CC213", "Programming Applications", 3, "CPE"),
        ("CC215", "Data Structures", 3, "CPE"),
        ("CC216", "Digital Logic Design", 3, "CPE"),
        ("CC218", "Discrete Mathematics", 3, "CPE"),
        ("CC311", "Computer Architecture", 3, "CPE"),
        ("CC312", "Computer Organization", 3, "CPE"),
        ("CC316", "Object-Oriented Programming", 3, "CPE"),
        ("CC317", "Digital System Design", 3, "CPE"),
        ("CC319", "Advanced Programming", 3, "CPE"),
        ("CC331", "Data & Computer Communications", 3, "CPE"),
        ("CC341", "Digital Electronics", 3, "CPE"),
        ("CC410", "Systems Programming", 3, "CPE"),
        ("CC411", "Introduction to Microprocessors", 3, "CPE"),
        ("CC413", "Numerical Analysis", 3, "CPE"),
        ("CC414", "Database Systems", 3, "CPE"),
        ("CC415", "Data Acquisition Systems", 3, "CPE"),
        ("CC416", "Computer Graphics", 3, "CPE"),
        ("CC418", "Operating Systems", 3, "CPE"),
        ("CC419", "Numerical Methods", 3, "CPE"),
        ("CC421", "Microprocessors Systems", 3, "CPE"),
        ("CC431", "Computer Networks", 3, "CPE"),
        ("CC501", "Graduation Project I", 3, "CPE"),
        ("CC511", "Artificial Intelligence", 3, "CPE"),
        ("CC513", "Computing Systems", 3, "CPE"),
        ("CC531", "Advanced Networks", 3, "CPE"),

        // ============================================================
        // Electronics & Communications (EC) — Engineering
        // ============================================================
        ("EC210", "Solid State Electronics", 3, "ECE"),
        ("EC217", "Measurements & Instrumentation", 3, "ECE"),
        ("EC218", "Measurements & Instrumentation", 3, "CPE"),
        ("EC233", "Electronic Devices (1)", 3, "ECE"),
        ("EC238", "Electronics I", 3, "CPE"),
        ("EC311", "Electronic Materials", 3, "ECE"),
        ("EC320", "Communication Theory", 3, "CPE"),
        ("EC321M", "Signals & Systems", 3, "ECE"),
        ("EC322M", "Introduction to Communication Systems", 3, "ECE"),
        ("EC332", "Electronic Devices (2)", 3, "ECE"),
        ("EC333", "Electronic Amplifiers", 3, "ECE"),
        ("EC334", "Analog & Digital Circuit Analysis", 3, "ECE"),
        ("EC339", "Electronics (2)", 3, "CPE"),
        ("EC341", "Electromagnetics", 3, "ECE"),
        ("EC410", "Electronic Measurements", 3, "ECE"),
        ("EC422", "Introduction to Digital Communications", 3, "ECE"),
        ("EC432", "Microelectronic Circuits", 3, "ECE"),
        ("EC434", "Analog Signal Processing", 3, "ECE"),
        ("EC442", "Electromagnetic Wave Propagation", 3, "ECE"),
        ("EC443", "Electromagnetic Transmitting Media", 3, "ECE"),
        ("EC533", "Digital Signal Processing", 3, "ECE"),
        ("EC544", "Antennas Engineering", 3, "ECE"),

        // ============================================================
        // Electrical Engineering (EE) — Engineering
        // ============================================================
        ("EE231", "Electrical Circuits (1)", 3, "ECE"),
        ("EE232", "Electrical Circuits (2)", 3, "ECE"),
        ("EE328", "Electrical Power and Machines", 3, "ECE"),
        ("EE418", "Automatic Control Systems", 3, "ECE"),
        ("EE419", "Modern Control Engineering", 3, "ECE"),

        // ============================================================
        // Industrial Management (IM)
        // ============================================================
        ("IM111", "Industrial Relations", 2, "BUA"),
        ("IM112", "Manufacturing Technology", 2, "BUA"),
        ("IM400", "Practical Training", 1, "BUA"),
        ("IM423", "Operations Research", 3, "BUA"),
        ("IM535", "International Operations Management", 3, "BUA"),

        // ============================================================
        // Languages & Humanities (LH)
        // ============================================================
        ("LH131", "English 1 / ESP 1", 2, "ECE"),
        ("LH132", "ESP 2", 2, "ECE"),
        ("LH231", "Technical Report Writing", 2, "ECE"),

        // ============================================================
        // Mechanical Engineering (ME)
        // ============================================================
        ("ME151", "Engineering Drawing and Projection", 2, "MRE"),

        // ============================================================
        // Non-Engineering (NE)
        // ============================================================
        ("NE264", "Scientific Thinking", 2, "ECE"),
        ("NE364", "Engineering Economy", 2, "ECE"),
        ("NE465", "Creative Awareness", 2, "ECE"),

        // ============================================================
        // Management College — Accounting (ACC)
        // ============================================================
        ("ACC101", "Financial Accounting I", 3, "ACC"),
        ("ACC102", "Financial Accounting II", 3, "ACC"),
        ("ACC201", "Managerial Accounting", 3, "ACC"),
        ("ACC202", "Cost Accounting", 3, "ACC"),
        ("ACC301", "Intermediate Accounting", 3, "ACC"),
        ("ACC302", "Auditing Principles", 3, "ACC"),
        ("ACC303", "Taxation", 3, "ACC"),
        ("ACC401", "Advanced Accounting", 3, "ACC"),
        ("ACC402", "Advanced Auditing", 3, "ACC"),
        ("ACC403", "Accounting Information Systems", 3, "ACC"),
        ("ACC501", "International Accounting Standards (IFRS)", 3, "ACC"),
        ("ACC502", "Forensic Accounting", 3, "ACC"),
        ("ACC503", "Financial Statement Analysis", 3, "ACC"),
        ("ACC601", "Tax Planning and Compliance", 3, "ACC"),
        ("ACC699", "Master's Thesis in Accounting", 6, "ACC"),

        // ============================================================
        // Management College — Business (BUS)
        // ============================================================
        ("BUS101", "Business Communication I", 2, "BUA"),
        ("BUS102", "Business Communication II", 2, "BUA"),
        ("BUS201", "Business Research Methods", 3, "BUA"),

        // ============================================================
        // Management College — Economics (ECO)
        // ============================================================
        ("ECO101", "Microeconomics", 3, "BUA"),
        ("ECO102", "Macroeconomics", 3, "BUA"),

        // ============================================================
        // Management College — Finance (FIN)
        // ============================================================
        ("FIN201", "Financial Management", 3, "BUA"),
        ("FIN301", "Corporate Finance", 3, "BUA"),

        // ============================================================
        // Management College — Law (LAW)
        // ============================================================
        ("LAW101", "Business Law", 2, "BUA"),

        // ============================================================
        // Management College — Management (MGT)
        // ============================================================
        ("MGT101", "Principles of Management", 3, "BUA"),
        ("MGT102", "Organizational Behavior", 3, "BUA"),
        ("MGT201", "Operations Management", 3, "BUA"),
        ("MGT202", "Human Resources Management", 3, "BUA"),
        ("MGT301", "Strategic Management", 3, "BUA"),
        ("MGT302", "Project Management", 3, "BUA"),
        ("MGT303", "International Business", 3, "BUA"),
        ("MGT304", "Entrepreneurship", 3, "BUA"),
        ("MGT401", "Leadership and Change Management", 3, "BUA"),
        ("MGT402", "Quality Management", 3, "BUA"),
        ("MGT403", "Supply Chain Management", 3, "BUA"),
        ("MGT501", "Business Consulting", 3, "BUA"),
        ("MGT599", "Master's Thesis in Management", 6, "BUA"),

        // ============================================================
        // Management College — MIS
        // ============================================================
        ("MIS201", "Management Information Systems", 3, "BUA"),

        // ============================================================
        // Management College — Marketing (MKT)
        // ============================================================
        ("MKT101", "Marketing Principles", 3, "BUA"),
        ("MKT301", "Advanced Marketing", 3, "BUA"),
        ("MKT401", "Digital Marketing", 3, "BUA"),

        // ============================================================
        // Management College — Statistics (STA)
        // ============================================================
        ("STA101", "Business Statistics", 3, "BUA"),
        ("STA201", "Quantitative Methods", 3, "BUA"),
    };
}
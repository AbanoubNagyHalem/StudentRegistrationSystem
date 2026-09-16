namespace StudentRegistration.Infrastructure.Data.Seed;

internal static class StudyPlanCatalog
{
  /// <summary>
  /// Mapping of StudyPlan (by department code) to (Term → CourseCodes[]).
  /// </summary>
  public static readonly Dictionary<string, Dictionary<int, string[]>> ByDepartment = new()
  {
    // ============================================================
    // Electronics and Communications Engineering (ECE)
    // ============================================================
    ["ECE"] = new()
    {
      [1] = new[] { "BA113", "BA123", "BA141", "CC111", "IM111", "LH131", "ME151" },
      [2] = new[] { "BA118", "BA124", "BA142", "CC112", "IM112", "LH132", "BA114" },
      [3] = new[] { "BA223", "CC213", "EC210", "EE231", "LH231", "NE465" },
      [4] = new[] { "BA224", "CC216", "EC217", "EC233", "EE232", "NE264" },
      [5] = new[] { "BA323", "CC312", "EC321M", "EC332", "EC334", "EE328" },
      [6] = new[] { "CC413", "EC311", "EC322M", "EC333", "EC341" },
      [7] = new[] { "CC411", "EC432", "EC442", "EE418", "IM423" },
      [8] = new[] { "EC410", "EC422", "EC434", "EC443", "EE419", "IM400", "NE364" },
      [9] = new[] { "EC544" },
      [10] = new[] { "EC533" },
    },

    // ============================================================
    // Computer Engineering (CPE)
    // ============================================================
    ["CPE"] = new()
    {
      [1] = new[] { "BA113", "BA118", "BA123", "BA141", "CC111", "IM111", "LH131" },
      [2] = new[] { "BA124", "BA142", "CC112", "IM112", "LH132", "ME151", "BA114" },
      [3] = new[] { "BA223", "CC212", "CC218", "EE231", "LH231" },
      [4] = new[] { "BA224", "CC215", "CC216", "EC218", "EC238", "EE232" },
      [5] = new[] { "BA323", "CC317", "CC319", "EC320", "EC339", "EE328" },
      [6] = new[] { "BA326", "CC311", "CC316", "CC331", "CC341", "NE364" },
      [7] = new[] { "CC410", "CC414", "CC419", "CC421", "EE418" },
      [8] = new[] { "CC415", "CC416", "CC418", "CC431", "IM400", "IM423" },
      [9] = new[] { "CC501", "CC511", "CC531" },
      [10] = new[] { "CC513", "IM535" },
    },

    // ============================================================
    // Business Administration (BUA) — Master's
    // ============================================================
    ["BUA"] = new()
    {
      [1] = new[] { "MGT101", "STA101", "MKT101", "ACC101", "BUS101", "ECO101" },
      [2] = new[] { "MGT102", "ECO102", "LAW101", "ACC102", "BUS102", "FIN201" },
      [3] = new[] { "MGT201", "MGT202", "ACC201", "BUS201", "MIS201" },
      [4] = new[] { "MGT301", "MGT302", "MGT303", "MGT304", "MKT301" },
      [5] = new[] { "MGT401", "MGT402", "STA201", "FIN301" },
      [6] = new[] { "MGT403", "MKT401", "IM423" },
      [7] = new[] { "MGT501" },
      [8] = new[] { "MGT599" },
    },

    // ============================================================
    // Accounting (ACC) — Master's
    // ============================================================
    ["ACC"] = new()
    {
      [1] = new[] { "ACC101", "STA101", "ECO101", "MGT101", "BUS101", "LAW101" },
      [2] = new[] { "ACC102", "ACC201", "ECO102", "ACC202", "BUS102" },
      [3] = new[] { "ACC301", "ACC302", "ACC303", "FIN201", "BUS201" },
      [4] = new[] { "ACC401", "ACC402", "MGT303", "ACC403" },
      [5] = new[] { "ACC501", "ACC502", "ACC503" },
      [6] = new[] { "ACC601", "MGT301" },
      [7] = new[] { "MGT402" },
      [8] = new[] { "ACC699" },
    },
  };

  /// <summary>
  /// Prerequisites: CourseCode → PrerequisiteCourseCodes[].
  /// </summary>
  public static readonly Dictionary<string, string[]> Prerequisites = new()
  {
    // ============================================================
    // Engineering prerequisites
    // ============================================================
    ["BA114"] = new[] { "BA113" },
    ["BA124"] = new[] { "BA123" },
    ["BA142"] = new[] { "BA141" },
    ["BA223"] = new[] { "BA124" },
    ["BA224"] = new[] { "BA223" },
    ["BA323"] = new[] { "BA224" },
    ["BA326"] = new[] { "BA124" },

    ["CC112"] = new[] { "CC111" },
    ["CC212"] = new[] { "CC112" },
    ["CC213"] = new[] { "CC112" },
    ["CC215"] = new[] { "CC212" },
    ["CC216"] = new[] { "CC111" },
    ["CC218"] = new[] { "CC111" },
    ["CC311"] = new[] { "CC317" },
    ["CC312"] = new[] { "CC216" },
    ["CC316"] = new[] { "CC319" },
    ["CC317"] = new[] { "CC216" },
    ["CC319"] = new[] { "CC215" },
    ["CC331"] = new[] { "EC320" },
    ["CC341"] = new[] { "EC238" },
    ["CC410"] = new[] { "CC319" },
    ["CC411"] = new[] { "CC216" },
    ["CC413"] = new[] { "CC112", "BA224" },
    ["CC414"] = new[] { "CC319" },
    ["CC415"] = new[] { "CC421" },
    ["CC416"] = new[] { "CC319" },
    ["CC418"] = new[] { "CC410" },
    ["CC419"] = new[] { "CC112", "BA224" },
    ["CC421"] = new[] { "CC311" },
    ["CC431"] = new[] { "CC331" },
    ["CC511"] = new[] { "CC218", "CC319" },
    ["CC513"] = new[] { "CC418", "CC421" },
    ["CC531"] = new[] { "CC431" },

    ["EC210"] = new[] { "BA114", "BA118" },
    ["EC217"] = new[] { "EE231" },
    ["EC218"] = new[] { "EE231" },
    ["EC233"] = new[] { "EC210" },
    ["EC238"] = new[] { "EE231" },
    ["EC311"] = new[] { "EC210" },
    ["EC320"] = new[] { "BA224", "EE231" },
    ["EC321M"] = new[] { "BA224", "EE231" },
    ["EC322M"] = new[] { "EC321M" },
    ["EC332"] = new[] { "EC233", "EE232" },
    ["EC333"] = new[] { "EC332" },
    ["EC334"] = new[] { "EE232", "EC233" },
    ["EC339"] = new[] { "EC238" },
    ["EC341"] = new[] { "BA114", "BA224" },
    ["EC410"] = new[] { "EC432" },
    ["EC422"] = new[] { "EC322M" },
    ["EC432"] = new[] { "EC333" },
    ["EC434"] = new[] { "EC432" },
    ["EC442"] = new[] { "EC341" },
    ["EC443"] = new[] { "EC442" },
    ["EC533"] = new[] { "EC434" },
    ["EC544"] = new[] { "EC443" },

    ["EE231"] = new[] { "BA124" },
    ["EE232"] = new[] { "EE231" },
    ["EE328"] = new[] { "EE232" },
    ["EE418"] = new[] { "EE328", "BA323" },
    ["EE419"] = new[] { "EE418" },

    ["LH132"] = new[] { "LH131" },
    ["LH231"] = new[] { "LH132" },

    // ============================================================
    // Management College prerequisites
    // ============================================================
    // Accounting
    ["ACC102"] = new[] { "ACC101" },
    ["ACC201"] = new[] { "ACC101" },
    ["ACC202"] = new[] { "ACC102" },
    ["ACC301"] = new[] { "ACC102" },
    ["ACC302"] = new[] { "ACC301" },
    ["ACC303"] = new[] { "ACC101" },
    ["ACC401"] = new[] { "ACC301" },
    ["ACC402"] = new[] { "ACC302" },
    ["ACC403"] = new[] { "ACC201" },
    ["ACC501"] = new[] { "ACC301" },
    ["ACC502"] = new[] { "ACC302" },
    ["ACC503"] = new[] { "ACC301" },
    ["ACC601"] = new[] { "ACC303" },
    ["ACC699"] = new[] { "ACC501" },

    // Business
    ["BUS102"] = new[] { "BUS101" },
    ["BUS201"] = new[] { "BUS101", "BUS102" },

    // Economics
    ["ECO102"] = new[] { "ECO101" },

    // Finance
    ["FIN201"] = new[] { "ACC101" },
    ["FIN301"] = new[] { "FIN201" },

    // Management
    ["MGT102"] = new[] { "MGT101" },
    ["MGT201"] = new[] { "MGT101" },
    ["MGT202"] = new[] { "MGT101" },
    ["MGT301"] = new[] { "MGT201", "MGT202" },
    ["MGT302"] = new[] { "MGT201" },
    ["MGT303"] = new[] { "MGT201" },
    ["MGT304"] = new[] { "MGT101" },
    ["MGT401"] = new[] { "MGT301" },
    ["MGT402"] = new[] { "MGT201" },
    ["MGT403"] = new[] { "MGT201" },
    ["MGT501"] = new[] { "MGT301" },
    ["MGT599"] = new[] { "MGT501" },

    // Marketing
    ["MKT301"] = new[] { "MKT101" },
    ["MKT401"] = new[] { "MKT301" },

    // Statistics
    ["STA201"] = new[] { "STA101" },
  };
}
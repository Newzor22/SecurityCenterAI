import type { DashboardSummary, RecentAnalysis } from "@/types/api";

type DashboardApiResponse = Partial<{
  securityScore: number;
  score: number;
  totalAnalyses: number;
  analysesCount: number;
  recentAnalyses: unknown[];
  history: unknown[];
}>;

function asRecord(value: unknown): Record<string, unknown> {
  return value && typeof value === "object" ? (value as Record<string, unknown>) : {};
}

function toAnalysis(value: unknown, index: number): RecentAnalysis {
  const item = asRecord(value);
  const rawStatus = String(item.status ?? item.riskLevel ?? "unknown").toLowerCase();
  const status: RecentAnalysis["status"] =
    rawStatus === "safe" || rawStatus === "warning" || rawStatus === "danger"
      ? rawStatus
      : "unknown";

  return {
    id: String(item.id ?? index),
    title: String(item.title ?? item.name ?? `Analisis ${index + 1}`),
    status,
    createdAt: String(item.createdAt ?? item.date ?? ""),
    summary: String(item.summary ?? item.description ?? "Sin resumen disponible."),
  };
}

export function normalizeDashboard(data: DashboardApiResponse): DashboardSummary {
  const recent = Array.isArray(data.recentAnalyses)
    ? data.recentAnalyses
    : Array.isArray(data.history)
      ? data.history
      : [];

  return {
    securityScore:
      typeof data.securityScore === "number"
        ? data.securityScore
        : typeof data.score === "number"
          ? data.score
          : null,
    totalAnalyses:
      typeof data.totalAnalyses === "number"
        ? data.totalAnalyses
        : typeof data.analysesCount === "number"
          ? data.analysesCount
          : recent.length,
    recentAnalyses: recent.map(toAnalysis),
  };
}

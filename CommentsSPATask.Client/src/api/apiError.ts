import type { ApiResponse } from "../types/shared/apiResponse";

export function getApiErrorText(error: { name: string } | null | undefined) {
  const name = error?.name.trim();

  return name ? name : null;
}

export function getApiErrorFromMutationError(error: unknown) {
  if (!error || typeof error !== "object" || !("data" in error)) {
    return null;
  }

  const data = (error as { data?: unknown }).data;

  if (!data || typeof data !== "object" || !("error" in data)) {
    return null;
  }

  return (data as ApiResponse<unknown>).error;
}
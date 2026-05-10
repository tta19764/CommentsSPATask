const rawBaseUrl = import.meta.env.VITE_BASE_URL?.trim();

function ensureTrailingSlash(value: string): string {
  return value.endsWith("/") ? value : `${value}/`;
}

function trimSlashes(value: string): string {
  return value.replace(/^\/+|\/+$/g, "");
}

function requireEnv(name: string, value: string | undefined): string {
  const normalized = value?.trim();

  if (!normalized) {
    throw new Error(`Missing required frontend environment variable: ${name}`);
  }

  return normalized;
}

export function getApiBaseUrl(): string {
  const candidate = requireEnv("VITE_BASE_URL", rawBaseUrl);

  try {
    return ensureTrailingSlash(new URL(candidate).toString());
  } catch {
    throw new Error(`Invalid VITE_BASE_URL: ${candidate}`);
  }
}

export function buildApiUrl(path: string): string {
  return new URL(path, getApiBaseUrl()).toString();
}

export function getApiPath(
  name: string,
  envValue: string | undefined
): string {
  const normalized = trimSlashes(requireEnv(name, envValue));

  return `${normalized}/`;
}

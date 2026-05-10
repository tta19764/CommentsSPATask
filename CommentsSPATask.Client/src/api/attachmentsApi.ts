import { buildApiUrl, getApiPath } from "../utils/apiUrl";

const UPLOADS_ENDPOINT = getApiPath(
  "VITE_APP_UPLOADS_ENDPOINT",
  import.meta.env.VITE_APP_UPLOADS_ENDPOINT
);

const UPLOADS_URL = buildApiUrl(UPLOADS_ENDPOINT);

export function getAttachmentUrl(
    storedFileName: string
): string {
    return `${UPLOADS_URL}${storedFileName}`;
}

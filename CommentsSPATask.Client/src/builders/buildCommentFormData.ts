import type { CreateCommentRequest } from "../types/comments/createCommentRequestType";

export function buildCommentFormData(
    request: CreateCommentRequest
): FormData {
    const formData = new FormData();

    formData.append("userName", request.userName);
    formData.append("email", request.email);
    formData.append("text", request.text);
    formData.append("captchaId", request.captchaId);
    formData.append("captchaInput", request.captchaInput);

    if (request.homePage) {
        formData.append("homePage", request.homePage);
    }

    if (request.attachment) {
        formData.append("attachment", request.attachment);
    }

    return formData;
}

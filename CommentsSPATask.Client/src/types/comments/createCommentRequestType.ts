export interface CreateCommentRequest {
    userName: string;
    email: string;
    homePage?: string;
    text: string;
    captchaId: string;
    captchaInput: string;
    attachment?: File;
}

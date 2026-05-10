export interface Comment {
    id: string;
    parentId?: string | null;
    userName: string;
    email: string;
    homePage: string | null;
    text: string;
    createdAtUtc: string;
    attachments: Attachment[];
    replies?: Comment[];
}

export interface Attachment {
    id: string;
    originalFileName: string;
    storedFileName: string;
    contentType: "Image" | "TextFile";
}

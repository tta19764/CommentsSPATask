export interface ApiError {
    code: string;
    name: string;
}

export interface ApiResponse<TData, TMetadata = null> {
    data: TData | null;
    metadata: TMetadata | null;
    error: ApiError | null;
}

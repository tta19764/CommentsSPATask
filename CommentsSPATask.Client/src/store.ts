import { configureStore } from "@reduxjs/toolkit";
import { commentsApi } from "./api/commentsApi";
import { captchaApi } from "./api/captchasApi";

export const store = configureStore({
    reducer: {
        [commentsApi.reducerPath]: commentsApi.reducer,
        [captchaApi.reducerPath]: captchaApi.reducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(
            commentsApi.middleware,
            captchaApi.middleware
        ),
});

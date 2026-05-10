import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import type { CreateCaptchaResponse } from "../types/captchas/createCaptchaResponse";
import { getApiBaseUrl, getApiPath } from "../utils/apiUrl";

const CAPTCHAS_ENDPOINT = getApiPath(
  "VITE_APP_CAPTCHAS_ENDPOINT",
  import.meta.env.VITE_APP_CAPTCHAS_ENDPOINT
);

export const captchaApi = createApi({
    reducerPath: "captchaApi",

    baseQuery: fetchBaseQuery({
        baseUrl: getApiBaseUrl(),
    }),

    tagTypes: ["Captcha"],

    endpoints: (builder) => ({
        createCaptcha: builder.mutation<
            CreateCaptchaResponse,
            void
        >({
            query: () => ({
                url: CAPTCHAS_ENDPOINT,
                method: "POST",
            }),
        }),

        getCaptchaImage: builder.query<
            Blob,
            string
        >({
            query: (captchaId) => ({
                url: `${CAPTCHAS_ENDPOINT}/${captchaId}/image`,
                responseHandler: (response) =>
                    response.blob(),
            }),
        }),
    }),
});

export const {
    useCreateCaptchaMutation,
    useGetCaptchaImageQuery,
} = captchaApi;

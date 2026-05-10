FROM node:22-alpine AS build
WORKDIR /app

ARG VITE_BASE_URL
ARG VITE_APP_COMMENTS_ENDPOINT
ARG VITE_APP_CAPTCHAS_ENDPOINT
ARG VITE_APP_UPLOADS_ENDPOINT

ENV VITE_BASE_URL=$VITE_BASE_URL
ENV VITE_APP_COMMENTS_ENDPOINT=$VITE_APP_COMMENTS_ENDPOINT
ENV VITE_APP_CAPTCHAS_ENDPOINT=$VITE_APP_CAPTCHAS_ENDPOINT
ENV VITE_APP_UPLOADS_ENDPOINT=$VITE_APP_UPLOADS_ENDPOINT

COPY ["CommentsSPATask.Client/package.json", "CommentsSPATask.Client/package-lock.json", "./"]
RUN npm ci

COPY CommentsSPATask.Client/. .
RUN npm run build

FROM nginx:1.27-alpine AS final
WORKDIR /usr/share/nginx/html

COPY CommentsSPATask.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist ./

EXPOSE 80

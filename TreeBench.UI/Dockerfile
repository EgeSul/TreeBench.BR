FROM node:20-alpine AS build
WORKDIR /app

COPY ["TreeBench.UI/package.json", "TreeBench.UI/package-lock.json*", "./"]
RUN npm install

COPY TreeBench.UI/ .
RUN npm run build

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/dist/ /usr/share/nginx/html/

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
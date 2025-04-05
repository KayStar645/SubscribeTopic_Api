# dotNetCleanArchitechure

## GET /GetPosts
```
?sorts=     LikeCount,CommentCount,-created         // sort by likes, then comments, then descendingly by date created 
&filters=   LikeCount>10, Title@=awesome title,     // filter to posts with more than 10 likes, and a title that contains the phrase "awesome title"
&page=      1                                       // get the first page...
&pageSize=  10                                      // ...which contains 10 posts
```

## Build Docker Image từ Dockerfile

```
docker build -t subscribe-topic-api .
```

##  Chạy API bằng Docker container

```
docker run -d -p 5000:5000 --name docker-subscribe-topic-api subscribe-topic-api
```
http://localhost:5000/swagger

## Dừng container (nếu cần)
```
docker stop docker-subscribe-topic-api
docker rm docker-subscribe-topic-api
```
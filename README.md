# multiple-ip-demo

### View available IP addresses of the container
![image](https://user-images.githubusercontent.com/117949398/204560766-70eac4ed-cb64-4941-87a7-1feb751632c2.png)

### Add more networks to the container via 
```
docker network create NETWORK_NAME
docker network connect NETWORK_NAME WebApplication1
```
### POST to webhook url on all IPs
https://webhook.site is a great place to get an instant hook to POST to 

![image](https://user-images.githubusercontent.com/117949398/204561427-e226b94e-5e03-49e2-97db-1e1ebd242bfe.png)
![image](https://user-images.githubusercontent.com/117949398/204561567-8a3b88d5-f8d4-4c70-ac8d-0a364039de05.png)

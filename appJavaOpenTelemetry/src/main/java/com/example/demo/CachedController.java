// src/main/java/com/example/demo/CachedController.java
package com.example.demo;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.client.RestTemplate;

import java.util.concurrent.TimeUnit;

@Controller
@RequestMapping("/cached")
public class CachedController {

    private final String USER_API1 = "https://jsonplaceholder.typicode.com/users/";
    private final String USER_API2 = "https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8";

    @Autowired
    private RestTemplate restTemplate;

    @Autowired
    private RedisTemplate<String, Object> redisTemplate;

    @GetMapping("/api1")
    @ResponseBody
    public ResponseEntity<Object> api1() {
        Object cachedData = redisTemplate.opsForValue().get("api1-cached");
        if (cachedData != null) {
            return ResponseEntity.ok(cachedData);
        } else {
            ResponseEntity<Object> responseEntity = restTemplate.getForEntity(USER_API1, Object.class);
            redisTemplate.opsForValue().set("api1-cached", responseEntity.getBody(), 300, TimeUnit.SECONDS);
            return responseEntity;
        }
    }

    @GetMapping("/api2")
    @ResponseBody
    public ResponseEntity<Object> api2() {
        Object cachedData = redisTemplate.opsForValue().get("api2-cached");
        if (cachedData != null) {
            return ResponseEntity.ok(cachedData);
        } else {
            ResponseEntity<Object> responseEntity = restTemplate.getForEntity(USER_API2, Object.class);
            redisTemplate.opsForValue().set("api2-cached", responseEntity.getBody(), 5, TimeUnit.SECONDS);
            return responseEntity;
        }
    }
}

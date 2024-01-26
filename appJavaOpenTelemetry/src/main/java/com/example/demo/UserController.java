// src/main/java/com/example/demo/UserController.java
package com.example.demo;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.client.RestTemplate;

@Controller
@RequestMapping("/api")
public class UserController {

    private final String USER_API1 = "https://jsonplaceholder.typicode.com/users/";
    private final String USER_API2 = "https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8";

    @Autowired
    private RestTemplate restTemplate;

    @GetMapping("/api1")
    @ResponseBody
    public ResponseEntity<Object> api1() {
        return restTemplate.getForEntity(USER_API1, Object.class);
    }

    @GetMapping("/api2")
    @ResponseBody
    public ResponseEntity<Object> api2() {
        return restTemplate.getForEntity(USER_API2, Object.class);
    }
}

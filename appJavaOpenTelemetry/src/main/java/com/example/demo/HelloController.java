// src/main/java/com/example/demo/HelloController.java
package com.example.demo;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/")
public class HelloController {

    @GetMapping
    public String helloWorld() {
        return "Hello World";
    }

    @GetMapping("/page1")
    public String helloPage1() {
        return "Hello Page1";
    }

    @GetMapping("/page2")
    public String helloPage2() {
        return "Hello Page2";
    }

    @GetMapping("/healthcheck")
    public String healthCheck() {
        return "ok";
    }

    @GetMapping("/errortest")
    public String errorTest() {
        throw new RuntimeException("Internal Server Error");
    }
}

const express = require("express");
const Redis = require('ioredis');
const axios = require('axios');

const PORT = process.env.PORT || "3000";
const app = express();
const USER_API1 = 'https://jsonplaceholder.typicode.com/users/'; //fake public api
const USER_API2 = 'https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8' //fake public api

const redis = new Redis();

app.get("/", (req, res) => {
  res.send("Hello World");
});

app.get("/page1", (req, res) => {
  res.send("Hello Page1");
});

app.get("/page2", (req, res) => {
  res.send("Hello Page2");
});

app.get("/healthcheck", (req, res) => {
  res.send("ok");
});

app.get("/errortest", (req, res) => {
  res.send(500);
});

app.get(`/api1`, async (req, res) => {
  try {
    const response = await axios.get(USER_API1);
    const data = response.data;
    res.send([{ url: data }]);
  } catch (err) {
    console.error(err);
    res.status(500).send("Erro interno do servidor");
  }
});

app.get(`/api2`, async (req, res) => {
  try {
    const response = await axios.get(USER_API2);
    const data = response.data;
    res.send([{ url: data }]);
  } catch (err) {
    console.error(err);
    res.status(500).send("Erro interno do servidor");
  }
});

app.get(`/api1-cached`, async (req, res) => {
  try {
    const cachedData = await redis.get('api1-cached');

    if (cachedData) {
      res.send([{ url: JSON.parse(cachedData) }]);
    } else {
      const response = await fetch(USER_API1);
      const data = await response.json();

      await redis.set('api1-cached', JSON.stringify(data), 'EX', 300);

      res.send([{ url: data }]);
    }
  } catch (err) {
    console.error(err);
    res.status(500).send("Erro interno do servidor");
  }
});

app.get(`/api2-cached`, async (req, res) => {
  try {
    const cachedData = await redis.get('api2-cached');

    if (cachedData) {
      res.send([{ url: JSON.parse(cachedData) }]);
    } else {
      const response = await fetch(USER_API2);
      const data = await response.json();

      await redis.set('api2-cached', JSON.stringify(data), 'EX', 5);

      res.send([{ url: data }]);
    }
  } catch (err) {
    console.error(err);
    res.status(500).send("Erro interno do servidor");
  }
});

app.listen(parseInt(PORT, 10), () => {
  console.log(`Listening for requests on http://localhost:${PORT}`);
});

process.on('SIGINT', async () => {
  await redis.quit();
  process.exit(0);
});


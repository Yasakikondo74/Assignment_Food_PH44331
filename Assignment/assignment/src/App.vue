<script setup>
import { provide } from 'vue'
import Navbar from './components/Navbar.vue'
import { RouterView } from 'vue-router'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import mitt from 'mitt'

const url = "https://localhost:7135"
provide("url", url)

const emitter = mitt()
provide("emitter", emitter)
</script>

<template>
  <Navbar />
  <div class="container-fluid">
    <RouterView />
  </div>
</template>

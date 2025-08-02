<script setup>
import Navbar from './components/Navbar.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.min.js'
import { RouterView } from 'vue-router'
import { provide } from 'vue'
const url = "https://localhost:7135"
provide("url", url)
</script>

<template>
	<Navbar />
	<div class="container-fluid">
		<RouterView />
	</div>
</template>
<template>
  <div class="d-flex flex-wrap gap-3 p-3">
    <div class="card" style="width: 18rem;" v-for="x in food_data" :key="x.id">
      <img :src="x.imageLink" class="card-img-top" :alt="x.foodName || 'Food image'" @error="handleImageError" />
      <div class="card-body">
        <h2>{{ x.foodName }}</h2>
        <p class="badge rounded-pill text-bg-success" v-if="x.available">Available!</p>
        <p class="badge text-bg-danger" v-else>Not Available</p>
        <p>{{ (x.cost - 0.01).toFixed(2) }} $</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue'
import axios from 'axios'

axios.defaults.withCredentials = true

const url = inject("url")
const food_data = ref([])

async function callAPI() {
    let res = await axios.get(url + '/food')
    food_data.value = res.data
}

onMounted( async () => {
    callAPI()
})

function handleImageError(event) {
  event.target.src = '/assets/not_found.png' 
}
</script>
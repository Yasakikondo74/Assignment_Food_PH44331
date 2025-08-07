<template>
  <div class="p-3">
    <input 
      type="text" 
      v-model="searchQuery" 
      placeholder="Search for food..." 
      class="form-control mb-3"
    />

    <div class="d-flex flex-wrap gap-3">
      <div class="card" style="width: 18rem;" v-for="x in filteredFoodItems" :key="x.id">
        <img :src="x.imageLink" class="card-img-top" :alt="x.foodName || 'Food image'" @error="handleImageError" />
        <div class="card-body">
          <h2>{{ x.foodName }}</h2>
          <p class="badge rounded-pill text-bg-success" v-if="x.available">Available!</p>
          <p class="badge text-bg-danger" v-else>Not Available</p>
          <p>{{ (x.cost - 0.01).toFixed(2) }} $</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted, computed } from 'vue'
import axios from 'axios'

axios.defaults.withCredentials = true

const url = inject("url")
const food_data = ref([])
const searchQuery = ref('') // New ref to hold the search input value

async function callAPI() {
    let res = await axios.get(url + '/food')
    food_data.value = res.data
}

// A computed property that filters the food data
const filteredFoodItems = computed(() => {
  if (!searchQuery.value) {
    // If the search bar is empty, show all items
    return food_data.value
  }
  
  // Otherwise, filter the list based on the search query
  return food_data.value.filter(food =>
    food.foodName.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

onMounted( async () => {
    callAPI()
})

function handleImageError(event) {
  event.target.src = '/assets/not_found.png' 
}
</script>
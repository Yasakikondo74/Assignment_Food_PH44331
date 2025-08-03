<template>
  <table class="table table-striped table-hover">
    <thead>
      <tr>
        <th scope="col">ID</th>
        <th scope="col">Name</th>
        <th scope="col">Available</th>
        <th scope="col">Quantity</th>
        <th scope="col">Cost</th>
      </tr>
    </thead>
    <tbody>
          <tr v-for="x in food_data" :key="x.id">
              <td class="fw-bold">{{ x.id}}</td>
              

<div class="card" style="width: 18rem;">
  <img src="..." class="card-img-top" alt="...">
  <div class="card-body">
    <p class="card-text">Some quick example text to build on the card title and make up the bulk of the card’s content.</p>
  </div>
</div>


              <td>{{ x.foodName }}</td>
              <td>{{ x.available }}</td>
              <td>{{ x.quantity }}</td>
              <td>{{ x.cost }}</td>
          </tr>
    </tbody>
  </table>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue'
import axios from 'axios'

const url = inject("url")
const food_data = ref([])

async function callAPI() {
    let res = await axios.get(url + '/food')
    food_data.value = res.data
}

onMounted( async () => {
    callAPI()
})
</script>
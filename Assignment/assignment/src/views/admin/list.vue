<template>
  <table class="table table-striped table-hover">
    <thead>
      <tr>
        <th scope="col">ID</th>
        <th scope="col">Name</th>
        <th scope="col">Available</th>
        <th scope="col">Quantity</th>
        <th scope="col">Cost</th>
        <th scope="col">Functions</th>
      </tr>
    </thead>
    <tbody>
          <tr v-for="x in food_data" :key="x.id">
              <td class="fw-bold">{{ x.id}}</td>
              <td>{{ x.foodName }}</td>
              <td>{{ x.available }}</td>
              <td>{{ x.quantity }}</td>
              <td>{{ x.cost }}</td>
              <td>
                  <button class="btn btn-danger" @click="remove(x.id)">Delete</button> &nbsp;
                  <RouterLink :to="'/admin/food/update/' + x.id" class="btn btn-warning">Edit</RouterLink> &nbsp;
                  <RouterLink :to="'/admin/food/detail/' + x.id" class="btn btn-secondary">Show More Detail</RouterLink>
              </td>
          </tr>
    </tbody>
  </table>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import axios from 'axios'

const url = inject("url")
const food_data = ref([])

async function callAPI() {
    let res = await axios.get(url + '/food')
    food_data.value = res.data
}

async function remove(id){
    if(confirm("Are you sure you want to delete this?")){
        await axios.delete(url + "/food/" + id)
        alert("Successfully Deleted")
        callAPI()
    }
}

onMounted( async () => {
    callAPI()
})
</script>
<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Add Food</h5>
    </div>
    <div class="card-body">
      <form @submit.prevent="addFood">
        <div class="mb-3">
          <label class="form-label fw-bold">Image</label><br>
          <img 
            :src="food.imageLink || '/assets/not_found.png'" 
            :alt="food.foodName || 'Food image'" 
            @error="handleImageError"
            style="width: auto; height: 500px;"
          >
          <input type="text" class="form-control mt-2" v-model="food.imageLink" />
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Name</label>
          <input type="text" class="form-control" v-model="food.foodName" required />
        </div>

        <div class="mb-3">
          <label class="form-label d-block fw-bold">Availability</label>
          <div class="form-check form-check-inline">
            <input class="form-check-input" type="radio" id="availableYes" value="true" v-model="food.available">
            <label class="form-check-label" for="availableYes">Yes</label>
          </div>
          <div class="form-check form-check-inline">
            <input class="form-check-input" type="radio" id="availableNo" value="false" v-model="food.available">
            <label class="form-check-label" for="availableNo">No</label>
          </div>
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Quantity</label>
          <input type="number" class="form-control" v-model.number="food.quantity" min="0" required />
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Cost</label>
          <input type="number" class="form-control" v-model.number="food.cost" min="0" required />
        </div>
        <button class="btn btn-success" type="submit">Add</button>
        <RouterLink to="/admin/food" class="btn btn-secondary ms-2">Back</RouterLink>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, inject } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import axios from 'axios'

axios.defaults.withCredentials = true

const router = useRouter()
const url = inject("url")

const food = reactive({
  foodName: "",
  available: "true",
  quantity: 0,
  cost: 0,
  imageLink: ""
})

async function addFood() {
  try {
    const payload = { ...food, available: food.available === "true" }
    await axios.post(url + "/food", payload)
    alert("Added successfully")
    router.push("/admin/food")
  } catch (err) {
    console.error("Failed to add food:", err)
    alert("Failed to add food")
  }
}

function handleImageError(event) {
  event.target.src = '/assets/not_found.png'
}
</script>

<style lang="sass" scoped>
span
  font-size: 150%
</style>

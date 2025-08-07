<template>
  <div class="container">
    <h2 class="my-4">Menu</h2>
    <div v-if="loading" class="text-center">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else-if="foods.length" class="row row-cols-1 row-cols-md-3 g-4">
      <div v-for="food in foods" :key="food.id" class="col">
        <div class="card h-100">
          <img :src="food.imageLink" class="card-img-top" alt="food image" />
          <div class="card-body">
            <h5 class="card-title">{{ food.foodName }}</h5>
            <p class="card-text">{{ food.cost }}$</p>
            <div class="d-flex align-items-center mb-3">
              <label :for="'quantity-' + food.id" class="form-label me-2 mb-0">Quantity:</label>
              <input
                type="number"
                class="form-control me-2"
                style="width: 80px;"
                min="1"
                :max="food.quantity"
                v-model.number="quantities[food.id]"
                :id="'quantity-' + food.id"
              />
              <span class="text-muted"> ({{ food.quantity }} available) </span>
            </div>
            <button
              @click="addToCart(food)"
              class="btn btn-primary"
              :disabled="!food.available || !quantities[food.id] || quantities[food.id] > food.quantity"
            >
              Add to Cart
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-else class="alert alert-info" role="alert">
      No food items are currently available. Please check back later!
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject } from 'vue'
import axios from 'axios'

const url = inject('url')
const foods = ref([])
const quantities = ref({})
const loading = ref(true)

async function fetchFoods() {
  try {
    const res = await axios.get(`${url}/food/available`)
    foods.value = res.data.filter(food => food.available && food.quantity > 0)
    foods.value.forEach(food => {
      quantities.value[food.id] = 1
    })
  } catch (error) {
    console.error('Failed to fetch food data:', error)
  } finally {
    loading.value = false
  }
}

async function addToCart(food) {
  const quantity = quantities.value[food.id]
  if (!quantity || quantity > food.quantity || quantity <= 0) {
    alert(`Please enter a valid quantity between 1 and ${food.quantity}.`)
    return
  }

  try {
    // Prepare the data to send to the new purchase endpoint
    const purchaseData = {
      foodId: food.id,
      quantity: quantity
    }

    // Post the purchase data to the new endpoint
    const res = await axios.post(`${url}/purchase`, purchaseData)

    if (res.status === 201) {
      alert(`Successfully purchased ${quantity} x ${food.foodName}! Receipt ID: ${res.data.receiptId}`)
      // Optionally, refresh the food list to update quantities
      await fetchFoods()
    }
  } catch (error) {
    console.error('Failed to add to cart:', error)
    alert('Failed to complete the purchase. Please try again.')
  }
}

onMounted(fetchFoods)
</script>
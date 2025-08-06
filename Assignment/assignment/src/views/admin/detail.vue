<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Food Details</h5>
    </div>
    <div class="card-body">
      <form>
        <div class="mb-3">
          <label class="form-label fw-bold">Image</label><br>
          <img 
            :src="editedData.imageLink" 
            :alt="editedData.foodName || 'Food image'" 
            @error="handleImageError"
            style="width: auto; height: 500px;"
          >
          <input type="text" class="form-control mt-2" v-model="editedData.imageLink" @input="markChanged" />
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Name</label>
          <input type="text" class="form-control" v-model="editedData.foodName" @input="markChanged" />
        </div>

        <div class="mb-3">
          <label class="form-label d-block fw-bold">Availability</label>
          <div class="form-check form-check-inline">
            <input class="form-check-input" type="radio" id="availableYes" value="true" v-model="editedData.available" @change="markChanged">
            <label class="form-check-label" for="availableYes">Yes</label>
          </div>
          <div class="form-check form-check-inline">
            <input class="form-check-input" type="radio" id="availableNo" value="false" v-model="editedData.available" @change="markChanged">
            <label class="form-check-label" for="availableNo">No</label>
          </div>
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Quantity</label>
          <input type="number" class="form-control" v-model.number="editedData.quantity" @input="markChanged" />
        </div>

        <div class="mb-3">
          <label class="form-label fw-bold">Cost</label>
          <input type="number" class="form-control" v-model.number="editedData.cost" @input="markChanged" />
        </div>
      </form>
    </div>

    <div class="card-footer d-flex justify-content-between">
      <RouterLink to="/admin/food" class="btn btn-secondary">Back</RouterLink>

      <div v-if="hasChanges">
        <button class="btn btn-success me-2" @click="saveChanges">Save</button>
        <button class="btn btn-secondary" @click="discardChanges">Discard</button>
      </div>
      <div v-else>
        <button class="btn btn-danger me-2" @click="remove">Delete</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, inject, onMounted, computed } from 'vue'
import { RouterLink, useRouter, useRoute } from 'vue-router'
import axios from 'axios'

axios.defaults.withCredentials = true

const route = useRoute()
const router = useRouter()
const id = route.params.id
const url = inject("url")

const originalData = ref(null)
const editedData = reactive({
  foodName: "",
  available: "false",
  quantity: 0,
  cost: 0,
  imageLink: ""
})

async function callAPI() {
  try {
    const res = await axios.get(url + '/food/' + id)
    originalData.value = { ...res.data }
    Object.assign(editedData, res.data)
    editedData.available = String(editedData.available)
  } catch (err) {
    console.error("Error fetching food data:", err)
  }
}

onMounted(() => {
  callAPI()
})

const hasChanges = computed(() => {
  if (!originalData.value) return false
  return (
    originalData.value.foodName !== editedData.foodName ||
    String(originalData.value.available) !== String(editedData.available) ||
    Number(originalData.value.quantity) !== Number(editedData.quantity) ||
    Number(originalData.value.cost) !== Number(editedData.cost) ||
    originalData.value.imageLink !== editedData.imageLink
  )
})

function discardChanges() {
  if (originalData.value) {
    Object.assign(editedData, originalData.value)
    editedData.available = String(editedData.available)
  }
}

async function saveChanges() {
  try {
    const payload = { ...editedData, available: editedData.available === "true" }
    await axios.put(url + "/food/" + id, payload)
    alert("Saved successfully")
    await callAPI()
  } catch (err) {
    console.error("Failed to save changes:", err)
    alert("Failed to save changes")
  }
}

async function remove() {
  if (confirm("Are you sure you want to delete this?")) {
    try {
      await axios.delete(url + "/food/" + id)
      alert("Deleted successfully")
      router.push("/admin/food")
    } catch (err) {
      console.error("Failed to delete:", err)
      alert("Failed to delete item")
    }
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

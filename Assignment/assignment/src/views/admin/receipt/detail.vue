<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Receipt Details</h5>
    </div>
    <div class="card-body">
      <div class="mb-3">
        <label class="form-label fw-bold">Receipt ID</label>
        <p class="form-control-plaintext">{{ receipt.id }}</p>
      </div>

      <div class="mb-3">
        <label class="form-label fw-bold">Account ID</label>
        <p class="form-control-plaintext">{{ receipt.accountId }}</p>
      </div>

      <div class="mb-3">
        <label class="form-label fw-bold">Created At</label>
        <p class="form-control-plaintext">{{ new Date(receipt.createdAt).toLocaleString() }}</p>
      </div>

      <div class="mb-3">
        <label class="form-label fw-bold">Receipt Items</label>
        <ul v-if="receipt.receiptDetails && receipt.receiptDetails.length">
          <li v-for="item in receipt.receiptDetails" :key="item.id">
            Food ID: {{ item.foodId }}, Quantity: {{ item.quantity }}, Cost: {{ item.cost }}$
          </li>
        </ul>
        <p v-else>No items in this receipt.</p>
      </div>
    </div>

    <div class="card-footer d-flex justify-content-between">
      <RouterLink to="/admin/receipt" class="btn btn-secondary">Back</RouterLink>
      <button class="btn btn-danger" @click="remove">Delete Receipt</button>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue';
import { RouterLink, useRouter, useRoute } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const route = useRoute();
const router = useRouter();
const id = route.params.id;
const url = inject("url");

const receipt = ref({
  id: "",
  accountId: "",
  createdAt: null,
  receiptDetails: []
});

async function callAPI() {
  try {
    const res = await axios.get(url + '/receipt/' + id);
    receipt.value = res.data;
  } catch (err) {
    console.error("Error fetching receipt data:", err);
  }
}

async function remove() {
  if (confirm("Are you sure you want to delete this receipt?")) {
    try {
      await axios.delete(url + "/receipt/" + id);
      alert("Deleted successfully");
      router.push("/admin/receipt");
    } catch (err) {
      console.error("Failed to delete receipt:", err);
      alert("Failed to delete receipt");
    }
  }
}

onMounted(() => {
  callAPI();
});
</script>

<style lang="sass" scoped>
span
  font-size: 150%
</style>
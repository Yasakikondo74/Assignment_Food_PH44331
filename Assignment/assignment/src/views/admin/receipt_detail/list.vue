<template>
  <div class="container-fluid">
    <div class="row">
      <div class="col-12">
        <h5 class="mt-4 mb-3">Receipt Details (Items)</h5>
        <div class="card shadow-sm">
          <div class="card-body">
            <table class="table table-striped table-hover">
              <thead>
                <tr>
                  <th scope="col">ID</th>
                  <th scope="col">Food ID</th>
                  <th scope="col">Quantity</th>
                  <th scope="col">Total Cost</th>
                  <th scope="col">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="x in receiptDetailData" :key="x.id">
                  <td class="fw-bold">{{ x.id }}</td>
                  <td>{{ x.foodId }}</td>
                  <td>{{ x.quantity }}</td>
                  <td>{{ formatCurrency(x.totalCost) }}</td>
                  <td>
                    <button class="btn btn-danger btn-sm" @click="remove(x.id)">
                      Delete
                    </button>
                    </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue';
import { RouterLink, useRoute } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const url = inject('url');
const route = useRoute();
const receiptDetailData = ref([]);

// This is how you could pass a prop if this component were nested.
const receiptId = route.params.id;

async function callAPI() {
  try {
    // This assumes an endpoint that returns all receipt details for a given receipt ID
    const res = await axios.get(`${url}/receipt/${receiptId}/details`);
    receiptDetailData.value = res.data;
  } catch (error) {
    console.error('Failed to fetch receipt detail data:', error);
    alert('Failed to fetch receipt detail data.');
  }
}

function formatCurrency(amount) {
  if (amount === null || amount === undefined) return 'N/A';
  return `$${Number(amount).toFixed(2)}`;
}

async function remove(id) {
  if (confirm('Are you sure you want to delete this receipt item?')) {
    try {
      await axios.delete(`${url}/receipt-detail/${id}`);
      alert('Successfully Deleted');
      await callAPI(); // Refresh the list after deletion
    } catch (error) {
      console.error('Failed to delete receipt item:', error);
      alert('Failed to delete receipt item.');
    }
  }
}

onMounted(() => {
  if (receiptId) {
    callAPI();
  }
});
</script>
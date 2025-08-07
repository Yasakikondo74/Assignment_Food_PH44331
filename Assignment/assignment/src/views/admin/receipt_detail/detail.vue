<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Receipt Item Details</h5>
    </div>
    <div class="card-body">
      <form>
        <div class="mb-3">
          <label class="form-label fw-bold">Receipt Detail ID</label>
          <input type="text" class="form-control" :value="detailData.id" disabled />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Receipt ID</label>
          <input type="text" class="form-control" :value="detailData.receiptId" disabled />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Food ID</label>
          <input type="text" class="form-control" :value="detailData.foodId" disabled />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Quantity</label>
          <input type="number" class="form-control" :value="detailData.quantity" disabled />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Total Cost</label>
          <input
            type="text"
            class="form-control"
            :value="formatCurrency(detailData.totalCost)"
            disabled
          />
        </div>
      </form>
    </div>

    <div class="card-footer d-flex justify-content-between">
      <RouterLink :to="`/admin/receipt/detail/${detailData.receiptId}`" class="btn btn-secondary">
        Back to Receipt
      </RouterLink>
      <div>
        <button class="btn btn-danger me-2" @click="remove">Delete</button>
      </div>
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
const url = inject('url');

const detailData = ref({});

async function callAPI() {
  try {
    const res = await axios.get(`${url}/receipt-detail/${id}`);
    detailData.value = res.data;
  } catch (err) {
    console.error('Error fetching receipt detail data:', err);
    alert('Failed to fetch receipt detail data.');
  }
}

function formatCurrency(amount) {
  if (amount === null || amount === undefined) return 'N/A';
  return `$${Number(amount).toFixed(2)}`;
}

async function remove() {
  if (confirm('Are you sure you want to delete this receipt item?')) {
    try {
      const parentReceiptId = detailData.value.receiptId;
      await axios.delete(`${url}/receipt-detail/${id}`);
      alert('Receipt item deleted successfully');
      router.push(`/admin/receipt/detail/${parentReceiptId}`);
    } catch (err) {
      console.error('Failed to delete receipt item:', err);
      alert('Failed to delete receipt item.');
    }
  }
}

onMounted(() => {
  callAPI();
});
</script>
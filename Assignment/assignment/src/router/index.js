import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/main.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/login.vue')
    },
    {
      path: '/signup',
      name: 'signup',
      component: () => import('../views/signup.vue')
    },
    {
      path: '/client',
      name: 'client',
      children:[
        {
          path: 'food',
          name: 'food-list-client',
          component: () => import('../views/client/list.vue')
        },
        {
          path: 'detail/:id',
          name: 'food-detail-client',
          component: () => import('../views/client/detail.vue')
        }
      ]
    },
    {
      path: '/admin',
      name: 'admin',
      children:[
        {
          path: 'food',
          name: 'food-list-admin',
          component: () => import('../views/admin/list.vue')
        },
        {
          path: 'add',
          name: 'food-add-admin',
          component: () => import('../views/admin/add.vue')
        },
        {
          path: 'update/:id',
          name: 'food-update-admin',
          component: () => import('../views/admin/update.vue')
        },        {
          path: 'detail/:id',
          name: 'food-detail-admin',
          component: () => import('../views/admin/detail.vue')
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('../views/NotFound.vue'),
    },
  ],
})

export default router
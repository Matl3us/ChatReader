import { createWebHashHistory, createRouter } from 'vue-router'
import HomeView from './Views/HomeView.vue'
import ChatView from './Views/ChatView.vue'

const routes = [
  { path: '/', component: HomeView },
  { path: '/chat', component: ChatView }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

export default router

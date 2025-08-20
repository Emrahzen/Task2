import { configureStore } from '@reduxjs/toolkit'
import cartReducer from './slices/cartSlice'

const PERSIST_KEY = 'cart_state_v1'

function loadPreloadedState(): any | undefined {
  if (typeof window === 'undefined') return undefined
  try {
    const raw = localStorage.getItem(PERSIST_KEY)
    if (!raw) return undefined
    const parsed = JSON.parse(raw)
    if (parsed && typeof parsed === 'object' && parsed.items) {
      return { cart: parsed }
    }
    return undefined
  } catch {
    return undefined
  }
}

function persistCartState(getState: () => any) {
  if (typeof window === 'undefined') return
  try {
    const state = getState()
    const cart = state?.cart
    if (cart) {
      localStorage.setItem(PERSIST_KEY, JSON.stringify(cart))
    }
  } catch {
    // ignore write errors
  }
}

export const makeStore = () => {
  const store = configureStore({
    reducer: {
      cart: cartReducer
    },
    preloadedState: undefined
  })
  if (typeof window !== 'undefined') {
    store.subscribe(() => persistCartState(store.getState))
  }
  return store
}

export type AppStore = ReturnType<typeof makeStore>
export type AppDispatch = AppStore['dispatch']
export type RootState = ReturnType<AppStore['getState']>
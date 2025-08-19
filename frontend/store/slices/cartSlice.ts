import { createSlice, PayloadAction } from '@reduxjs/toolkit'

type CartItem = {
  id: number
  name: string
  price: number
  imageUrl?: string
  quantity?: number
}

type CartState = {
  items: CartItem[]
}

const initialState: CartState = {
  items: []
}

const cartSlice = createSlice({
  name: 'cart',
  initialState,
  reducers: {
    addItem(state, action: PayloadAction<CartItem>) {
      const existing = state.items.find(i => i.id === action.payload.id)
      if (existing) {
        existing.quantity = (existing.quantity || 1) + 1
      } else {
        state.items.push({ ...action.payload, quantity: 1 })
      }
    },
    removeItem(state, action: PayloadAction<number>) {
      state.items = state.items.filter(i => i.id !== action.payload)
    },
    decrementItem(state, action: PayloadAction<number>) {
      const existing = state.items.find(i => i.id === action.payload)
      if (existing) {
        existing.quantity = Math.max(0, (existing.quantity || 1) - 1)
        if (existing.quantity === 0) {
          state.items = state.items.filter(i => i.id !== action.payload)
        }
      }
    },
    clearCart(state) {
      state.items = []
    }
  }
})

export const { addItem, removeItem, decrementItem, clearCart } = cartSlice.actions
export default cartSlice.reducer
import { createSlice, PayloadAction } from '@reduxjs/toolkit'

type CartItem = {
  id: number
  name: string
  price: number
  imageUrl?: string
  quantity?: number
  stockQuantity?: number
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
    hydrate(state, action: PayloadAction<CartState>) {
      state.items = action.payload.items || []
    },
    addItem(state, action: PayloadAction<CartItem>) {
      const existing = state.items.find(i => i.id === action.payload.id)
      if (existing) {
        const maxQty = existing.stockQuantity ?? Infinity
        const current = existing.quantity || 1
        if (current < maxQty) {
          existing.quantity = current + 1
        }
      } else {
        const initialQty = (action.payload.stockQuantity ?? 0) > 0 ? 1 : 0
        if (initialQty > 0) {
          state.items.push({ ...action.payload, quantity: initialQty })
        }
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

export const { hydrate, addItem, removeItem, decrementItem, clearCart } = cartSlice.actions
export default cartSlice.reducer
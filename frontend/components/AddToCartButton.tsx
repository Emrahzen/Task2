"use client"
import { useAppDispatch } from '@/store/hooks'
import { addItem } from '@/store/slices/cartSlice'

export default function AddToCartButton({ product, locale = 'tr' }: { product: any, locale?: string }) {
  const dispatch = useAppDispatch()
  return (
    <button onClick={() => dispatch(addItem({ id: product.id, name: product.name, price: product.price, imageUrl: product.imageUrl }))} className="bg-black text-white px-6 py-3 rounded">
      {locale === 'tr' ? 'Sepete Ekle' : 'Add to Cart'}
    </button>
  )
}
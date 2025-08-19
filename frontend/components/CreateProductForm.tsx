"use client"
import { useState } from 'react'
import { Constants } from '@/lib/constants'

interface CreateProductFormData {
  name: string
  description: string
  price: number
  stockQuantity: number
  imageUrl: string
  category: string
  brand: string
}

export default function CreateProductForm({ locale = 'tr' }: { locale?: string }) {
  const [formData, setFormData] = useState<CreateProductFormData>({
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    imageUrl: '',
    category: '',
    brand: ''
  })
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState('')

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: name === 'price' || name === 'stockQuantity' ? parseFloat(value) || 0 : value
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    setMessage('')

    try {
      const token = localStorage.getItem('token')
      if (!token) {
        setMessage(locale === 'tr' ? 'Lütfen önce giriş yapın' : 'Please login first')
        return
      }

      const response = await fetch(Constants.getApiUrl(Constants.API_ENDPOINTS.PRODUCT.CREATE), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(formData)
      })

      if (response.ok) {
        const result = await response.json()
        setMessage(locale === 'tr' ? 'Ürün başarıyla eklendi!' : 'Product added successfully!')
        setFormData({
          name: '',
          description: '',
          price: 0,
          stockQuantity: 0,
          imageUrl: '',
          category: '',
          brand: ''
        })
      } else {
        const errorData = await response.json()
        setMessage(`Hata: ${errorData.message || (locale === 'tr' ? 'Ürün eklenirken bir hata oluştu' : 'An error occurred while adding the product')}`)
      }
    } catch (error) {
              setMessage(locale === 'tr' ? 'Bağlantı hatası oluştu' : 'Connection error occurred')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="max-w-2xl mx-auto p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-6 text-gray-900">
        {locale === 'tr' ? 'Yeni Ürün Ekle' : 'Add New Product'}
      </h2>
      
      {message && (
        <div className={`mb-4 p-3 rounded ${
          message.includes('başarıyla') 
            ? 'bg-green-100 text-green-700 border border-green-300' 
            : 'bg-red-100 text-red-700 border border-red-300'
        }`}>
          {message}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
            {locale === 'tr' ? 'Ürün Adı' : 'Product Name'} *
          </label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder={locale === 'tr' ? 'Ürün adını girin' : 'Enter product name'}
          />
        </div>

        <div>
          <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
            {locale === 'tr' ? 'Açıklama' : 'Description'}
          </label>
          <textarea
            id="description"
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            rows={3}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder={locale === 'tr' ? 'Ürün açıklaması girin' : 'Enter product description'}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label htmlFor="price" className="block text-sm font-medium text-gray-700 mb-1">
              {locale === 'tr' ? 'Fiyat' : 'Price'} *
            </label>
            <input
              type="number"
              id="price"
              name="price"
              value={formData.price}
              onChange={handleInputChange}
              required
              min="0"
              step="0.01"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="0.00"
            />
          </div>

          <div>
            <label htmlFor="stockQuantity" className="block text-sm font-medium text-gray-700 mb-1">
              {locale === 'tr' ? 'Stok Miktarı' : 'Stock Quantity'} *
            </label>
            <input
              type="number"
              id="stockQuantity"
              name="stockQuantity"
              value={formData.stockQuantity}
              onChange={handleInputChange}
              required
              min="0"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="0"
            />
          </div>
        </div>

        <div>
          <label htmlFor="imageUrl" className="block text-sm font-medium text-gray-700 mb-1">
            {locale === 'tr' ? 'Resim URL' : 'Image URL'}
          </label>
          <input
            type="url"
            id="imageUrl"
            name="imageUrl"
            value={formData.imageUrl}
            onChange={handleInputChange}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="https://example.com/image.jpg"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label htmlFor="category" className="block text-sm font-medium text-gray-700 mb-1">
              {locale === 'tr' ? 'Kategori' : 'Category'}
            </label>
            <input
              type="text"
              id="category"
              name="category"
              value={formData.category}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Elektronik"
            />
          </div>

          <div>
            <label htmlFor="brand" className="block text-sm font-medium text-gray-700 mb-1">
              {locale === 'tr' ? 'Marka' : 'Brand'}
            </label>
            <input
              type="text"
              id="brand"
              name="brand"
              value={formData.brand}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Apple"
            />
          </div>
        </div>

        <button
          type="submit"
          disabled={isLoading}
          className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? (locale === 'tr' ? 'Ekleniyor...' : 'Adding...') : (locale === 'tr' ? 'Ürün Ekle' : 'Add Product')}
        </button>
      </form>
    </div>
  )
} 
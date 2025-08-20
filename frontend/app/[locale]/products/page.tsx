"use client"
import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import Image from 'next/image'
import Link from 'next/link'
import { Constants } from '@/lib/constants'

interface Product {
  id: number
  name: string
  description: string
  price: number
  stockQuantity: number
  imageUrl: string
  category: string
  brand: string
  isActive: boolean
}



export default function ProductsPage({ params: { locale }, searchParams }: { params: { locale: string }, searchParams: Record<string, string | string[] | undefined> }) {
  const router = useRouter()
  const [products, setProducts] = useState<Product[]>([])
  const [filteredProducts, setFilteredProducts] = useState<Product[]>([])
  const [filters, setFilters] = useState({
    searchTerm: '',
    category: '',
    minPrice: '',
    maxPrice: '',
    sortBy: ''
  })
  
  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      router.push('/tr/auth/login')
      return
    }
    
    fetchProducts()
  }, [router])

  useEffect(() => {
    applyFilters()
  }, [filters, products])

  const fetchProducts = async () => {
    try {
      const res = await fetch(Constants.getApiUrl(Constants.API_ENDPOINTS.PRODUCT.GET_ALL))
      if (res.ok) {
        const data = await res.json()
        setProducts(data)
        setFilteredProducts(data)
      }
    } catch (error) {
    }
  }

  const handleDelete = async (id: number) => {
    const token = localStorage.getItem('token')
    if (!token) return
    if (!confirm(locale === 'tr' ? 'Bu ürünü silmek istediğinize emin misiniz?' : 'Are you sure you want to delete this product?')) return
    try {
      const res = await fetch(Constants.getApiUrl(Constants.API_ENDPOINTS.PRODUCT.DELETE(String(id))), {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      })
      if (res.ok) {
        setProducts(prev => prev.filter(p => p.id !== id))
        setFilteredProducts(prev => prev.filter(p => p.id !== id))
      } else {
        const txt = await res.text()
        console.error('Delete failed', res.status, txt)
        alert(locale === 'tr' ? 'Silme işlemi başarısız.' : 'Delete failed')
      }
    } catch (e) {
      console.error(e)
      alert(locale === 'tr' ? 'Bağlantı hatası' : 'Connection error')
    }
  }

  const applyFilters = () => {
    let result = [...products]

    if (filters.searchTerm) {
      result = result.filter(product => 
        product.name.toLowerCase().includes(filters.searchTerm.toLowerCase()) ||
        (product.description && product.description.toLowerCase().includes(filters.searchTerm.toLowerCase())) ||
        (product.category && product.category.toLowerCase().includes(filters.searchTerm.toLowerCase())) ||
        (product.brand && product.brand.toLowerCase().includes(filters.searchTerm.toLowerCase()))
      )
    }

    if (filters.category) {
      result = result.filter(product => 
        product.category && product.category.toLowerCase().includes(filters.category.toLowerCase())
      )
    }

    if (filters.minPrice) {
      const minPrice = parseFloat(filters.minPrice)
      if (!isNaN(minPrice)) {
        result = result.filter(product => product.price >= minPrice)
      }
    }

    if (filters.maxPrice) {
      const maxPrice = parseFloat(filters.maxPrice)
      if (!isNaN(maxPrice)) {
        result = result.filter(product => product.price <= maxPrice)
      }
    }

    if (filters.sortBy) {
      switch (filters.sortBy) {
        case 'price_asc':
          result = result.sort((a, b) => a.price - b.price)
          break
        case 'price_desc':
          result = result.sort((a, b) => b.price - a.price)
          break
        case 'name_asc':
          result = result.sort((a, b) => a.name.localeCompare(b.name))
          break
        case 'name_desc':
          result = result.sort((a, b) => b.name.localeCompare(a.name))
          break
        default:
          break
      }
    }

    setFilteredProducts(result)
  }

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFilters(prev => ({
      ...prev,
      [name]: value
    }))
  }

  const handleFilterSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    applyFilters()
  }

  const clearFilters = () => {
    setFilters({
      searchTerm: '',
      category: '',
      minPrice: '',
      maxPrice: '',
      sortBy: ''
    })
  }



  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">
          {locale === 'tr' ? 'Ürünler' : 'Products'}
        </h1>
        <Link
          href={`/${locale}/products/create`}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          {locale === 'tr' ? 'Yeni Ürün Ekle' : 'Add New Product'}
        </Link>
      </div>
      
      <form onSubmit={handleFilterSubmit} className="mb-6 grid grid-cols-2 md:grid-cols-5 gap-3">
        {(filters.searchTerm || filters.category || filters.minPrice || filters.maxPrice || filters.sortBy) && (
          <div className="col-span-2 md:col-span-5 mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
            <div className="text-sm text-blue-800">
              <strong>{locale === 'tr' ? 'Aktif Filtreler:' : 'Active Filters:'}</strong>
              {filters.searchTerm && <span className="ml-2 px-2 py-1 bg-blue-100 rounded">{locale === 'tr' ? 'Arama' : 'Search'}: {filters.searchTerm}</span>}
              {filters.category && <span className="ml-2 px-2 py-1 bg-blue-100 rounded">{locale === 'tr' ? 'Kategori' : 'Category'}: {filters.category}</span>}
              {filters.minPrice && <span className="ml-2 px-2 py-1 bg-blue-100 rounded">{locale === 'tr' ? 'Min Fiyat' : 'Min Price'}: ₺{filters.minPrice}</span>}
              {filters.maxPrice && <span className="ml-2 px-2 py-1 bg-blue-100 rounded">{locale === 'tr' ? 'Max Fiyat' : 'Max Price'}: ₺{filters.maxPrice}</span>}
              {filters.sortBy && <span className="ml-2 px-2 py-1 bg-blue-100 rounded">{locale === 'tr' ? 'Sıralama' : 'Sort'}: {filters.sortBy}</span>}
              <span className="ml-2 text-blue-600">
                ({filteredProducts.length} {locale === 'tr' ? 'ürün bulundu' : 'products found'})
              </span>
            </div>
          </div>
        )}
        <input 
          name="searchTerm" 
          value={filters.searchTerm}
          onChange={handleFilterChange}
          placeholder={locale === 'tr' ? 'Ara...' : 'Search...'} 
          className="border px-3 py-2 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" 
        />
        <input 
          name="category" 
          value={filters.category}
          onChange={handleFilterChange}
          placeholder={locale === 'tr' ? 'Kategori' : 'Category'} 
          className="border px-3 py-2 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" 
        />
        <input 
          name="minPrice" 
          value={filters.minPrice}
          onChange={handleFilterChange}
          type="number"
          step="0.01"
          placeholder={locale === 'tr' ? 'Min Fiyat' : 'Min Price'} 
          className="border px-3 py-2 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" 
        />
        <input 
          name="maxPrice" 
          value={filters.maxPrice}
          onChange={handleFilterChange}
          type="number"
          step="0.01"
          placeholder={locale === 'tr' ? 'Max Fiyat' : 'Max Price'} 
          className="border px-3 py-2 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" 
        />
        <select 
          name="sortBy" 
          value={filters.sortBy}
          onChange={handleFilterChange}
          className="border px-3 py-2 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        >
          <option value="">{locale === 'tr' ? 'Sırala' : 'Sort By'}</option>
          <option value="price_asc">{locale === 'tr' ? 'Fiyat: Düşükten Yükseğe' : 'Price: Low to High'}</option>
          <option value="price_desc">{locale === 'tr' ? 'Fiyat: Yüksekten Düşüğe' : 'Price: High to Low'}</option>
          <option value="name_asc">{locale === 'tr' ? 'İsim: A\'dan Z\'ye' : 'Name: A to Z'}</option>
          <option value="name_desc">{locale === 'tr' ? 'İsim: Z\'den A\'ya' : 'Name: Z to A'}</option>
        </select>
        <button 
          type="submit"
          className="col-span-2 md:col-span-1 bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          {locale === 'tr' ? 'Filtrele' : 'Filter'}
        </button>
        <button 
          type="button"
          onClick={clearFilters}
          className="col-span-2 md:col-span-1 bg-gray-500 text-white px-6 py-2 rounded-lg hover:bg-gray-600 transition-colors"
        >
          {locale === 'tr' ? 'Filtreleri Temizle' : 'Clear Filters'}
        </button>
      </form>

      {filteredProducts.length === 0 ? (
        <div className="text-center py-12">
          <div className="text-gray-500 text-lg">
            {products.length === 0 
              ? (locale === 'tr' ? "Henüz ürün bulunamadı." : "No products found yet.")
              : (locale === 'tr' ? "Filtrelere uygun ürün bulunamadı." : "No products match the filters.")
            }
          </div>
          <div className="text-gray-400 text-sm mt-2">
            {products.length === 0 
              ? (locale === 'tr' ? "İlk ürünü eklemek için 'Yeni Ürün Ekle' butonuna tıklayın." : "Click 'Add New Product' to add your first product.")
              : (locale === 'tr' ? "Farklı filtreler deneyin veya filtreleri temizleyin." : "Try different filters or clear the filters.")
            }
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {filteredProducts.map((p: Product) => (
            <Link key={p.id} href={`/${locale}/products/${p.id}`} className="group">
              <div className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300">
                <div className="relative h-48">
                  <Image 
                    src={Constants.getImageUrl(p.imageUrl)} 
                    alt={p.name} 
                    fill 
                    className="object-cover group-hover:scale-105 transition-transform duration-300" 
                  />
                </div>
                <div className="p-4">
                  <div className="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors line-clamp-2 mb-2">
                    {p.name}
                  </div>
                  <div className="text-sm text-gray-600 mb-2">{p.category || '-'}</div>
                  <div className="text-lg font-bold text-blue-600">₺{p.price}</div>
                  <div className="text-sm text-gray-500">{locale === 'tr' ? 'Stok' : 'Stock'}: {p.stockQuantity}</div>
                  <div className="mt-3 flex justify-end">
                    <button onClick={(e) => { e.preventDefault(); handleDelete(p.id) }} className="text-red-600 hover:text-red-700 text-sm">
                      {locale === 'tr' ? 'Sil' : 'Delete'}
                    </button>
                  </div>
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
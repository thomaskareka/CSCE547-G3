import './App.css';
import { Route, Routes } from 'react-router-dom';
import Home from './organisms/Home/home';
import ParkDetails from './organisms/ParkDetails/parkDetails';
import Cart from './organisms/Cart/cart'
import ParkService from './services/parkService';
import CartService from './services/cartService';
import { useEffect, useState } from 'react';
import Homebar from './components/Homebar/homebar';
import Footer from './components/Footer/footer';
import { useMemo } from 'react';
import { CartItem } from './models/cartItem';

function App() {

  const parkService = useMemo(() => new ParkService(), []);
  const cartService = useMemo(() => new CartService(), []);
  const [cart, setCart] = useState<CartItem[]>([]);

  const handleChange = async () => {
    const maybe = cartService.loadCart();
    const resolved = (maybe instanceof Promise) ? await maybe : maybe;
    setCart(resolved ?? []);
  }

  useEffect(() => {
    let mounted = true;
    const load = async () => {
      const maybe = cartService.loadCart();
      const resolved = (maybe instanceof Promise) ? await maybe : maybe;
      if (mounted) setCart(resolved ?? []);
    };
    load();
    return () => { mounted = false; };
  }, []);

  return (
      <div className="App">
        <div className="header content">
          <Homebar numItems={cart ? cart.length : 0} />
        </div>
        <Routes>
          <Route path="/*" element={<Home parkService={parkService} cartService={cartService} />} />
          <Route path="details/:parkId" element={<ParkDetails parkService={parkService} cartService={cartService} onBook={handleChange} />} />
		      <Route path="/cart" element={<Cart cartService={cartService} handleChange={handleChange} /> } />
        </Routes>
        <div className="footer content">
          <Footer />
        </div>
      </div>
  );
}

export default App;

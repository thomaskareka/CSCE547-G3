import CartService from "../../services/cartService";
import CartCard from "../cartCard/cartCard";
import { CartItem } from '../../models/cartItem';
import { useEffect, useState } from "react";
import "./cartDetails.css"
import PaymentDetails from "../PaymentDetails/PaymentDetails";

type CartDetailsProps = {
	cartService: CartService
    handleDelete: () => void
}

export default function CartDetails(props: CartDetailsProps) {
	const { cartService, handleDelete } = props;

    //Pulling from local storage as source of truth
    const [cart, setCart] = useState<CartItem[]>([]);
    const [paymentOption, setPaymentOption] = useState("PAY_AT_PARK");

    useEffect(() => {
    const load = async () => {
        const result = await cartService.loadCart();
        setCart(result ?? []);
    };
    load();
    }, []);
	

    const updateCartItem = async (newCartItem: CartItem) => {
        const existing = cart.find((ci) => ci.park.id === newCartItem.park.id);
        cartService.updateCart(existing ?? newCartItem, newCartItem);
        const res = cartService.loadCart();
        const resolved = res instanceof Promise ? await res : res;
        setCart(resolved ?? []);
    }

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const v = e.target.value as "PAY_AT_PARK" | "PAY_NOW";
        setPaymentOption(v);
        }

    const deleteCartItem = async (item: CartItem) => {
        cartService.removeItemFromCart(item);
        handleDelete();
        const res = cartService.loadCart();
        const resolved = res instanceof Promise ? await res : res;
        setCart(resolved ?? []);
    }

    const getTaxPrice = () => {
        return cart.reduce((acc, curr) => {
            const {numAdults, numDays, numKids, park} = curr;
            return (
                acc + 
                    ((numAdults * numDays * park.adultPrice) + 
                    (numKids * numDays * park.childPrice)) * 0.08 
        )
        }, 0)
    }

    const getTotalPrice = () => {
        return cart.reduce((acc, curr) => {
            const {numAdults, numDays, numKids, park} = curr;
            return (
                acc + 
                    ((numAdults * numDays * park.adultPrice) + 
                    (numKids * numDays * park.childPrice)) * 1.08 
        )
        }, 0)
    }
	
    return(
        <div>
            <div className="cartItems column">
                {cart.map(((item: CartItem) => <CartCard cartItem={item} updateFn={(e) => updateCartItem(e)} deleteFn={deleteCartItem} />))}      
            </div>
            <div>
                Tax: ${getTaxPrice().toFixed(2)}
            </div>
            <div>
                Total Price: ${getTotalPrice().toFixed(2)}
            </div>
            <label>
                How would you like to pay?
                <input type="radio" name="selectedPayment" value={"PAY_AT_PARK"} checked={paymentOption === "PAY_AT_PARK"} onChange={handleChange} /> Pay Later at the Park
                <input type="radio" name="selectedPayment" value={"PAY_NOW"} checked={paymentOption === "PAY_NOW"} onChange={handleChange} /> Pay Now
            </label>
            {
                paymentOption === "PAY_NOW" && <PaymentDetails />
            }
        </div>
    )
}
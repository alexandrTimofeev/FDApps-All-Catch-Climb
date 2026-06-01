using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class CustomersLine : MonoBehaviour
{
    [SerializeField] private int customersAmount;
    [SerializeField] private float spacing;
    [Header("Components")]
    [SerializeField] private Transform startingTR;
    [SerializeField] private Transform customerPrefab;
    private FishObjectPool fishObjectPool;
    private Queue<Customer> customersQueue = new Queue<Customer>();
    private Customer currentCustomer;
    private bool isVIP_line = false;

    [Inject]
    public void Construct(FishObjectPool fishObjectPool)
    {
        this.fishObjectPool = fishObjectPool;
    }

    public void Awake()
    {
        if (fishObjectPool == null)
        {
            Debug.LogError($"{nameof(CustomersLine)}.{nameof(Awake)}: {nameof(fishObjectPool)} is null on {name}.");
            return;
        }

        if (startingTR == null)
        {
            Debug.LogError($"{nameof(CustomersLine)}.{nameof(Awake)}: {nameof(startingTR)} is null on {name}.");
            return;
        }

        if (customerPrefab == null)
        {
            Debug.LogError($"{nameof(CustomersLine)}.{nameof(Awake)}: {nameof(customerPrefab)} is null on {name}.");
            return;
        }

        InitializeLine();
    }
    private void InitializeLine()
    {
        for (int i = 0; i < customersAmount; i++)
        {
            Vector3 position = startingTR.position + Vector3.back * spacing * i;

            var customerGO = Instantiate(customerPrefab, position, Quaternion.identity);
            var customerScript = customerGO.GetComponent<Customer>();
            if (customerScript == null)
            {
                Debug.LogError($"{nameof(CustomersLine)}.{nameof(InitializeLine)}: {nameof(Customer)} component is null on {customerGO.name}.");
                continue;
            }

            customerScript.SetValues(this, fishObjectPool);

            customersQueue.Enqueue(customerScript);
        }

        if (customersQueue.Count > 0)
            currentCustomer = customersQueue.Peek();
    }
    public void NextCustomer()
    {
        currentCustomer = customersQueue.Peek();
    }
    public void SellToCurrentCustomer(FishItem fishItem, Action callback)
    {
        Action action = callback + DelayedUpdateCustomersPosition;

        Customer customer = currentCustomer;

        currentCustomer = null;
        customersQueue.Dequeue();

        customer.BuyProduct(fishItem, action);
    }
    public void AddNewCustomer(Customer customer)
    {
        customersQueue.Enqueue(customer);
    }
    private void DelayedUpdateCustomersPosition()
    {
        DOVirtual.DelayedCall(0.6f, () => UpdateCustomerPosition());
    }
    private void UpdateCustomerPosition() // Updates customers position in line
    {
        //TODO: Optimize
        List<Customer> customersList = customersQueue.ToList();

        for (int i = 0; i < customersQueue.Count; i++)
        {
            Vector3 position = startingTR.position + Vector3.back * spacing * i;

            Customer customer = customersList[i];

            customer.SetMoving(true);
            customer.transform.DOMove(position, 1f);
            DOVirtual.DelayedCall(0.8f, () => customer.SetMoving(false));
        }

        // Cals NextCustomer after all customers moved in line
        if (currentCustomer == null) DOVirtual.DelayedCall(1.05f, () => NextCustomer());
    }
    public void ReturnCustomer(Customer customer)
    {
        if (!isVIP_line) return;

        int i = UnityEngine.Random.Range(0, 100);

        if (i <= 35) // Chance of spawning VIP customer
        {
            customer.MakeVIP();
        }
    }
    public bool HasCurrentCustomer() => currentCustomer != null;
    public void UnlockVIP()
    {
        isVIP_line = true;
    }
}

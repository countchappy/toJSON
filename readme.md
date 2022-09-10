Currently Broken - No plans to fix

toJSON allows you to take input data and format it in a JSON format given a template.



Example (test_input.xlsx):
```
physician_id	physician_name	order_id	product_id	product_name	order_dt_tm	active_product	serial_number
198353	MD Jane Brown	7662983	15773	Drug A	10/10/2020	1	1234
198353	MD Jane Brown	7662983	15773	Drug A	10/10/2020	1	2345
198353	MD Jane Brown	7662983	15773	Drug A	10/10/2020	1	3456
198353	MD Jane Brown	7662983	15773	Drug A	10/10/2020	1	4567
198353	MD Jane Brown	7593872	22897	Bandages A	9/23/2020	0	4321
198353	MD Jane Brown	7593872	22897	Bandages A	9/23/2020	0	5432
198353	MD Jane Brown	7593872	22897	Bandages A	9/23/2020	0	6543
198353	MD Jane Brown	7593872	22897	Bandages A	9/23/2020	0	7654
198355	NP John Doe	7872147	12983	Drug B	11/1/2020	1	1627
198355	NP John Doe	7872147	12983	Drug B	11/1/2020	1	2536
198355	NP John Doe	7872147	12983	Drug B	11/1/2020	1	3445
198355	NP John Doe	7872147	12983	Drug B	11/1/2020	1	4534
198355	NP John Doe	7721513	32458	Bandages B	10/28/2020	0	6712
198355	NP John Doe	7721513	32458	Bandages B	10/28/2020	0	5623
198355	NP John Doe	7721513	32458	Bandages B	10/28/2020	0	4534
198355	NP John Doe	7721513	32458	Bandages B	10/28/2020	0	3445
```

Template:
```json
{
    "${header-0}": "${int(column-0)}",
    "${header-1}": "${column-1}",
    "${group=[orders]}": [{
        "${header-2}": "${int(column-2)}",
        "${header-3}": "${int(column-3)}",
        "${header-4}": "${column-4}",
        "${header-5}": "${datetime(column-5)}",
        "${header-6}": "${bool[1](column-6)}",
        "${group=[serial_numbers]}": [{
            "${int(column-${I})}"
        }]
    }]
}
```

Output (command> toJSON --ie xlsx --o out.json --i test_input.xlsx --otf template.json):
```json
[
    {
        "physician_id": 198353,
        "physician_name": "MD Jane Brown",
        "orders": [
            {
                "order_id": 7662983,
                "product_id": 15773,
                "product_name": "Drug A",
                "order_dt_tm": "2020-10-10T00:00:00",
                "active_product": true,
                "serial_numbers": [1234, 2345, 3456, 4567]
            }, 
            {
                "order_id": 7593872,
                "product_id": 22897,
                "product_name": "Bandages A",
                "order_dt_tm": "2020-09-23T00:00:00",
                "active_product": true,
                "serial_numbers": [4321, 5432, 6543, 7654]
            }
        ]
    },
    {
        "physician_id": 198355,
        "physician_name": "NP John Doe",
        "orders": [
            {
                "order_id": 7872147,
                "product_id": 12983,
                "product_name": "Drug B",
                "order_dt_tm": "2020-11-01T00:00:00",
                "active_product": true,
                "serial_numbers": [1627, 2536, 3445, 4534]
            }, 
            {
                "order_id": 7721513,
                "product_id": 32458,
                "product_name": "Bandages B",
                "order_dt_tm": "2020-10-28T00:00:00",
                "active_product": false,
                "serial_numbers": [6712, 5623, 4534, 3445]
            }
        ]
    }
]
```
